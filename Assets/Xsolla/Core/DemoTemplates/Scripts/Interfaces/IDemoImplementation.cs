using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

public interface IDemoImplementation
{
	void GetCatalogVirtualItems([NotNull] Action<List<CatalogVirtualItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetCatalogVirtualCurrencies([NotNull] Action<List<CatalogVirtualCurrencyModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	List<string> GetCatalogGroupsByItem(CatalogItemModel item);

	void GetInventoryItems([NotNull] Action<List<InventoryItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetVirtualCurrencyBalance([NotNull] Action<List<VirtualCurrencyBalanceModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void ConsumeInventoryItem(InventoryItemModel item, uint count, [NotNull] Action<InventoryItemModel> onSuccess,
		[CanBeNull] Action<InventoryItemModel> onFailed = null);

	void PurchaseForRealMoney(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	void PurchaseForVirtualCurrency(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
}