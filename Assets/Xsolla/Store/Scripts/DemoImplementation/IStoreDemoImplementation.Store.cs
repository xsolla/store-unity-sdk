using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial interface IStoreDemoImplementation
	{
		void PurchaseCart(List<UserCartItem> items, [NotNull] Action<List<UserCartItem>> onSuccess,
			[CanBeNull] Action<Error> onError = null, bool isSetPreviousDemoState = true);

		void PurchaseForRealMoney(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void PurchaseForVirtualCurrency(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null, bool isConfirmationRequired = true, bool isShowResultToUser = true);
	}
}
