using System;

namespace Xsolla.Demo
{
	public interface IStoreService
	{
		event Action<PurchaseInfo> OnPurchaseSuccess;

		void PurchaseItem(string sku, Action onSuccess, Action<string> onError);

		void PurchaseForVirtualCurrency(string sku, string priceSku, Action onSuccess, Action<string> onPurchaseError);
	}
}