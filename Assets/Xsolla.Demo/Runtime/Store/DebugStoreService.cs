#if DEBUG_XSOLLA_DEMO
using System;

namespace Xsolla.Demo
{
	public class DebugStoreService : IStoreService
	{
		private readonly InventoryStorage InventoryStorage;

		public DebugStoreService(InventoryStorage inventoryStorage)
		{
			InventoryStorage = inventoryStorage;
		}

		public event Action<PurchaseInfo> OnPurchaseSuccess;

		public void PurchaseItem(string sku, Action onSuccess, Action<string> onError)
		{
			var newQuantity = InventoryStorage.GetItem(sku).Quantity + 1;
			InventoryStorage.UpdateQuantity(sku, newQuantity);

			OnPurchaseSuccess?.Invoke(new PurchaseInfo(sku));
			onSuccess?.Invoke();
		}

		public void PurchaseForVirtualCurrency(string sku, string priceSku, Action onSuccess, Action<string> onPurchaseError)
		{
			var newQuantity = InventoryStorage.GetItem(sku).Quantity + 1;
			InventoryStorage.UpdateQuantity(sku, newQuantity);

			OnPurchaseSuccess?.Invoke(new PurchaseInfo(sku));
			onSuccess?.Invoke();
		}
	}
}
#endif