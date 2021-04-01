using System;

namespace Xsolla.Demo
{
	public class BattlePassPromocodeRewarder : BaseBattlePassRewarder
	{
		public override void CollectReward(BattlePassItemDescription itemDescription, Action onSuccess, Action onError)
		{
			DemoController.Instance.InventoryDemo.RedeemCouponCode(itemDescription.Promocode, _ => onSuccess?.Invoke(), _ => onError?.Invoke());
		}
	}
}
