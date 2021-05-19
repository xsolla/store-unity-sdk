using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	public void PurchaseForRealMoney(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, item.Sku, data =>
		{
			XsollaStore.Instance.OpenPurchaseUi(data);
			XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
			{
				PurchaseComplete(item);
				if (onSuccess != null)
					onSuccess.Invoke(item);
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
				item.VirtualPrice.Value.Key, _ =>
				{
					PurchaseComplete(item);
					if (onSuccess != null)
						onSuccess.Invoke(item);
				}, WrapErrorCallback(onError));
		});
	}

	public void PurchaseCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError = null)
	{
		if (!items.Any())
		{
			var error = new Error(errorMessage: "Cart is empty");
			var errorToInvoke = WrapErrorCallback(onError);
			if (errorToInvoke != null)
				errorToInvoke.Invoke(error);
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
							browser.BrowserClosedEvent += _ =>
							{
								if (onError != null)
									onError.Invoke(null);
							};
#endif

						XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
						{
							PurchaseComplete(null, () => DemoController.Instance.SetPreviousState());
							if (onSuccess != null)
								onSuccess.Invoke(items);
							UserCart.Instance.Clear();
						}, WrapErrorCallback(onError));
					}, WrapErrorCallback(onError));
				}, WrapErrorCallback(onError));
			}, WrapErrorCallback(onError));
		}, WrapErrorCallback(onError));
	}

	private static void PurchaseComplete(CatalogItemModel item = null, Action popupButtonCallback = null)
	{
		UserInventory.Instance.Refresh();
#if (UNITY_EDITOR || UNITY_STANDALONE)
		CloseInGameBrowserIfExist();
#endif
		if(item != null)
			StoreDemoPopup.ShowSuccess(string.Format("You are purchased '{0}'", item.Name));
		else
			StoreDemoPopup.ShowSuccess(null, popupButtonCallback);
	}
#if (UNITY_EDITOR || UNITY_STANDALONE)	
	private static void CloseInGameBrowserIfExist()
	{
		if(BrowserHelper.Instance.GetLastBrowser() != null)
			Destroy(BrowserHelper.Instance, 0.1F);
	}
#endif
}
