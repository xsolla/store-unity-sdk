using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToPremiumBuyerProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassPremiumBuyer PremiumBuyer;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			PremiumBuyer.OnBattlePassDescriptionArrived(battlePassDescription);
		}
	}
}
