using System;
using Xsolla.Catalog;

namespace Xsolla.Demo
{
	public class XsollaStoreService : IStoreService
	{
		public event Action<PurchaseInfo> OnPurchaseSuccess;

		public void PurchaseItem(string sku, Action onSuccess, Action<string> onError)
		{
			XsollaCatalog.Purchase(
				sku,
				_ => {
					OnPurchaseSuccess?.Invoke(new PurchaseInfo(sku));
					onSuccess?.Invoke();
				},
				error => onError?.Invoke(error.errorMessage));
		}

		public void PurchaseForVirtualCurrency(string sku, string priceSku, Action onSuccess, Action<string> onPurchaseError)
		{
			XsollaCatalog.PurchaseForVirtualCurrency(
				sku,
				priceSku,
				_ => {
					OnPurchaseSuccess?.Invoke(new PurchaseInfo(sku));
					onSuccess?.Invoke();
				},
				error => onPurchaseError?.Invoke(error.errorMessage));
		}
	}
}