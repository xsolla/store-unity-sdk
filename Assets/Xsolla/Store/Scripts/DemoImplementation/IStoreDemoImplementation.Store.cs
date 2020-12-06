using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial interface IStoreDemoImplementation
	{
		void GetCatalogVirtualItems([NotNull] Action<List<CatalogVirtualItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

		void GetCatalogSubscriptions([NotNull] Action<List<CatalogSubscriptionItemModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void GetCatalogVirtualCurrencyPackages([NotNull] Action<List<CatalogVirtualCurrencyModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void GetCatalogBundles([NotNull] Action<List<CatalogBundleItemModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void PurchaseCart(List<UserCartItem> items, [NotNull] Action<List<UserCartItem>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void PurchaseForRealMoney(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void PurchaseForVirtualCurrency(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);
	}
}
