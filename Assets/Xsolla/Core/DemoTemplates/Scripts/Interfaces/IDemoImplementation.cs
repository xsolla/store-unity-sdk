using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

public interface IDemoImplementation
{
	void GetVirtualCurrencies([NotNull] Action<List<VirtualCurrencyModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);
	
	void GetCatalogVirtualItems([NotNull] Action<List<CatalogVirtualItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetCatalogVirtualCurrencyPackages([NotNull] Action<List<CatalogVirtualCurrencyModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);
	
	void GetCatalogSubscriptions([NotNull] Action<List<CatalogSubscriptionItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	List<string> GetCatalogGroupsByItem(CatalogItemModel item);

	void GetInventoryItems([NotNull] Action<List<InventoryItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetVirtualCurrencyBalance([NotNull] Action<List<VirtualCurrencyBalanceModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);
	
	void GetUserSubscriptions([NotNull] Action<List<UserSubscriptionModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void ConsumeInventoryItem(InventoryItemModel item, uint count, [NotNull] Action<InventoryItemModel> onSuccess,
		[CanBeNull] Action<InventoryItemModel> onFailed = null);

	void PurchaseForRealMoney(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	void PurchaseForVirtualCurrency(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	void PurchaseCart(List<UserCartItem> items, [NotNull] Action<List<UserCartItem>> onSuccess, 
		[CanBeNull] Action<Error> onError = null);
}