using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial interface IInventoryDemoImplementation
	{
		void GetInventoryItems([NotNull] Action<List<InventoryItemModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void GetVirtualCurrencyBalance([NotNull] Action<List<VirtualCurrencyBalanceModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void GetUserSubscriptions([NotNull] Action<List<UserSubscriptionModel>> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void ConsumeInventoryItem(InventoryItemModel item, int? count, [NotNull] Action<InventoryItemModel> onSuccess,
			[CanBeNull] Action<InventoryItemModel> onFailed = null, bool isConfirmationRequired = true);

		void RedeemCouponCode(string couponCode, [NotNull] Action<List<CouponRedeemedItemModel>> onSuccess, [CanBeNull] Action<Error> onError);
	}
}
