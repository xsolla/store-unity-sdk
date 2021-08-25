using System;

namespace Xsolla.Demo
{
	public class BattlePassPromocodeRewarder : BaseBattlePassRewarder
	{
		public override void CollectReward(BattlePassItemDescription itemDescription, Action onSuccess, Action onError)
		{
			DemoInventory.Instance.RedeemCouponCode(itemDescription.Promocode, _ => onSuccess?.Invoke(), _ => onError?.Invoke());
		}
	}
}
