using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class CouponsManager : MonoBehaviour
	{
		[SerializeField] private SimpleTextButton redeemCouponButton = default;

		void Start()
		{
			redeemCouponButton.onClick += ShowRedeemCouponPopup;
		}

		private void ShowRedeemCouponPopup()
		{
			var redeemCouponPopup = PopupFactory.Instance.CreateRedeemCoupon();
			redeemCouponPopup.SetRedeemCallback(code =>
			{
				DemoInventory.Instance.RedeemCouponCode(code, redeemedItems =>
				{
					redeemCouponPopup.Close();
					UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
					PopupFactory.Instance.CreateCouponRewards().SetItems(redeemedItems);
				}, error => redeemCouponPopup.ShowError());
			});
		}
	}
}
