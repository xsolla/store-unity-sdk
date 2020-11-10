using UnityEngine;
using Xsolla.Core.Popup;

public class CouponsManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private MenuButton redeemCouponButton;
#pragma warning restore 0649

	void Start()
	{
		redeemCouponButton.onClick += s => { ShowRedeemCouponPopup(); };
	}

	private void ShowRedeemCouponPopup()
	{
		var redeemCouponPopup = PopupFactory.Instance.CreateRedeemCoupon();
		redeemCouponPopup.SetRedeemCallback(code =>
		{
			DemoController.Instance.GetImplementation().RedeemCouponCode(code, redeemedItems =>
			{
				
			}, error => redeemCouponPopup.ShowError());
		});
	}
}