using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoSingleton<DemoImplementation>, IDemoImplementation
{
	public void PurchaseForRealMoney(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, item.Sku, data =>
		{
			XsollaStore.Instance.OpenPurchaseUi(data);
			XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
			{
				UserInventory.Instance.Refresh();
				StoreDemoPopup.ShowSuccess();
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
					PurchaseForVirtualCurrencyComplete(item);
				}, WrapErrorCallback(onError));
		});
	}

	private void PurchaseForVirtualCurrencyComplete(CatalogItemModel item)
	{
		if (item.IsVirtualCurrency())
		{
			UserInventory.Instance.Refresh();
		}
		StoreDemoPopup.ShowSuccess($"You are purchased '{item.Name}'");
	}
}
