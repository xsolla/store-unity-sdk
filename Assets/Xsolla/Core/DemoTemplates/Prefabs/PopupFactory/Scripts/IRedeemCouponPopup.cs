using System;

namespace Xsolla.Core.Popup
{
	public interface IRedeemCouponPopup
	{
		IRedeemCouponPopup SetRedeemCallback(Action<string> buttonPressed);
		IRedeemCouponPopup SetCancelCallback(Action buttonPressed);
		IRedeemCouponPopup ShowError();
	}
}