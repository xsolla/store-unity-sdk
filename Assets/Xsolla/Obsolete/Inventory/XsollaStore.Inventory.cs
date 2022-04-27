using System;
using JetBrains.Annotations;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaInventory instead")]
		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int limit = 50, int offset = 0)
			=> XsollaInventory.Instance.GetInventoryItems(projectId, onSuccess, onError, limit, offset, locale);

		[Obsolete("Use XsollaInventory instead")]
		public void ConsumeInventoryItem(string projectId, ConsumeItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaInventory.Instance.ConsumeInventoryItem(projectId, item, onSuccess, onError);

		[Obsolete("Use XsollaCatalog instead")]
		public void RedeemCouponCode(string projectId, CouponCode couponCode, [NotNull] Action<CouponRedeemedItems> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCatalog.Instance.RedeemCouponCode(projectId, couponCode, onSuccess, onError);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetCouponRewards(string projectId, string couponCode, [NotNull] Action<CouponReward> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCatalog.Instance.GetCouponRewards(projectId, couponCode, onSuccess, onError);
	}
}
