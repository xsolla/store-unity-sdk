namespace Xsolla.Core.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public ICouponRedeemPopup CreateRedeemCoupon()
		{
			var popup = CreateDefaultPopup(RedeemCouponPopupPrefab, canvas);

			if (popup != null)
				return popup.GetComponent<CouponRedeemPopup>();
			else
				return null;
		}

		public ICouponRewardsPopup CreateCouponRewards()
		{
			var popup = CreateDefaultPopup(CouponRewardsPopupPrefab, canvas);

			if (popup != null)
				return popup.GetComponent<CouponRewardsPopup>();
			else
				return null;
		}
	}
}
