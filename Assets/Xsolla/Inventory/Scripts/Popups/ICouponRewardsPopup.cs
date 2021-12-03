using System.Collections.Generic;
using Xsolla.Demo;

namespace Xsolla.Core.Popup
{
	public interface ICouponRewardsPopup
	{
		ICouponRewardsPopup SetItems(List<CouponRedeemedItemModel> items);
		ISuccessPopup GetPopupCore();
	}
}