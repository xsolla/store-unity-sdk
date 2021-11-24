using System.Collections.Generic;
using UnityEngine;
using Xsolla.Demo;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/CouponRewardsPopup")]
	public class CouponRewardsPopup : MonoBehaviour, ICouponRewardsPopup
	{
		[SerializeField] private GameObject itemPrefab;
		[SerializeField] private ItemContainer itemsContainer;

		public ICouponRewardsPopup SetItems(List<CouponRedeemedItemModel> items)
		{
			items.ForEach(AddCouponRewardItem);
			return this;
		}

		private void AddCouponRewardItem(CouponRedeemedItemModel couponRewardItem)
		{
			itemsContainer.AddItem(itemPrefab).GetComponent<CouponRewardItemUI>().Initialize(couponRewardItem);
		}
	}
}
