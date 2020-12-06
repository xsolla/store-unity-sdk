using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
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
				XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
				{
					PurchaseComplete(item);
					onSuccess?.Invoke(item);
				}, WrapErrorCallback(onError));
			}, WrapErrorCallback(onError));
		}

		public void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			StoreDemoPopup.ShowConfirm(() =>
			{
				XsollaStore.Instance.ItemPurchaseForVirtualCurrency(
					XsollaSettings.StoreProjectId, 
					item.Sku, 
					item.VirtualPrice?.Key, _ =>
					{
						PurchaseComplete(item);
						onSuccess?.Invoke(item);
					}, WrapErrorCallback(onError));
			});
		}

		public void PurchaseCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError = null)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				var errorToInvoke = WrapErrorCallback(onError);
				errorToInvoke?.Invoke(error);
				return;
			}


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
						XsollaStore.Instance.CartPurchase(XsollaSettings.StoreProjectId, newCart.cart_id, data =>
						{
							XsollaStore.Instance.OpenPurchaseUi(data);

	#if (UNITY_EDITOR || UNITY_STANDALONE)
							var browser = BrowserHelper.Instance.GetLastBrowser();
							if (browser != null)
								browser.BrowserClosedEvent += _ => onError?.Invoke(null);
	#endif

							XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
							{
								PurchaseComplete();
								onSuccess?.Invoke(items);
								UserCart.Instance.Clear();
							}, WrapErrorCallback(onError));
						}, WrapErrorCallback(onError));
					}, WrapErrorCallback(onError));
				}, WrapErrorCallback(onError));
			}, WrapErrorCallback(onError));
		}

		private static void PurchaseComplete(CatalogItemModel item = null)
		{
			UserInventory.Instance.Refresh();
	#if (UNITY_EDITOR || UNITY_STANDALONE)
			CloseInGameBrowserIfExist();
	#endif
			if(item != null)
				StoreDemoPopup.ShowSuccess($"You are purchased '{item.Name}'");
			else
				StoreDemoPopup.ShowSuccess();
		}
	#if (UNITY_EDITOR || UNITY_STANDALONE)	
		private static void CloseInGameBrowserIfExist()
		{
			if(BrowserHelper.Instance.GetLastBrowser() != null)
				Destroy(BrowserHelper.Instance, 0.1F);
		}
	#endif
	}
}
