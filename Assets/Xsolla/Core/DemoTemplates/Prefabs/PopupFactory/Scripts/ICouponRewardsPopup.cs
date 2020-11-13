using System.Collections.Generic;

namespace Xsolla.Core.Popup
{
	public interface ICouponRewardsPopup
	{
		ICouponRewardsPopup SetItems(List<CouponRedeemedItemModel> items);
	}
}