using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoImplementation : MonoBehaviour, IStoreDemoImplementation
	{
		public void PurchaseForRealMoney(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, item.Sku, data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				XsollaStore.Instance.AddOrderForTracking(XsollaSettings.StoreProjectId, data.order_id, () =>
				{
					PurchaseComplete(item);
					onSuccess?.Invoke(item);
				}, WrapErrorCallback(onError));
			}, WrapErrorCallback(onError));
		}

		public void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null,
			bool isConfirmationRequired = true, bool isShowResultToUser = true)
		{
			var onConfirmation = new Action( () =>
			{
				XsollaStore.Instance.ItemPurchaseForVirtualCurrency(
					XsollaSettings.StoreProjectId, 
					item.Sku, 
					item.VirtualPrice?.Key, _ =>
					{
						PurchaseComplete(item, isShowResultToUser: isShowResultToUser);
						onSuccess?.Invoke(item);
					}, WrapErrorCallback(onError));
			});

			if (isConfirmationRequired)
				StoreDemoPopup.ShowConfirm(onConfirmation);
			else
				onConfirmation?.Invoke();
		}

		public void PurchaseCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError = null, bool isSetPreviousDemoState = true, bool isShowResultToUser = true)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				var errorToInvoke = WrapErrorCallback(onError);
				errorToInvoke?.Invoke(error);
				return;
			}

			var isCartUnlocked = false;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isCartUnlocked);

			XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart =>
			{
				XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, newCart.cart_id, () =>
				{
					var cartItems = items.Select(i => new CartFillItem
					{
						sku = i.Sku,
						quantity = i.Quantity
					}).ToList();
					XsollaStore.Instance.FillCart(XsollaSettings.StoreProjectId, cartItems, () =>
					{
						isCartUnlocked = true;

						XsollaStore.Instance.CartPurchase(XsollaSettings.StoreProjectId, newCart.cart_id, data =>
						{
							XsollaStore.Instance.OpenPurchaseUi(data);

	#if (UNITY_EDITOR || UNITY_STANDALONE)
							var browser = BrowserHelper.Instance.GetLastBrowser();
							if (browser != null)
								browser.BrowserClosedEvent += _ => onError?.Invoke(null);
	#endif

							XsollaStore.Instance.AddOrderForTracking(XsollaSettings.StoreProjectId, data.order_id, () =>
							{
								PurchaseComplete(null, () =>
								{
									if (isSetPreviousDemoState)
										DemoController.Instance.SetPreviousState();
								}, isShowResultToUser);
								onSuccess?.Invoke(items);
								UserCart.Instance.Clear();
							}, WrapErrorCallback(onError));
						}, WrapErrorCallback(onError));
					}, WrapErrorCallback(error => { isCartUnlocked = true; onError?.Invoke(error); }));
				}, WrapErrorCallback(error => { isCartUnlocked = true; onError?.Invoke(error); }));
			}, WrapErrorCallback(error => { isCartUnlocked = true; onError?.Invoke(error); }));
		}

		private static void PurchaseComplete(CatalogItemModel item = null, Action popupButtonCallback = null, bool isShowResultToUser = true)
		{
			UserInventory.Instance.Refresh();
			if (BrowserHelper.Instance.GetLastBrowser() != null)
			{
#if (UNITY_EDITOR || UNITY_STANDALONE)
				BrowserHelper.Instance.GetLastBrowser().BrowserClosedEvent += browser =>
				{
					ShowPurchaseCompleteMessage(item, popupButtonCallback, isShowResultToUser);
				};
#endif
			}
			else
			{
				ShowPurchaseCompleteMessage(item, popupButtonCallback, isShowResultToUser);
			}
		}

		private static void ShowPurchaseCompleteMessage(CatalogItemModel item = null, Action popupButtonCallback = null, bool isShowResultToUser = true)
		{
			if (isShowResultToUser)
			{
				if(item != null)
					StoreDemoPopup.ShowSuccess($"You have purchased '{item.Name}'");
				else
					StoreDemoPopup.ShowSuccess(null, popupButtonCallback);
			}
		}
	}
}
