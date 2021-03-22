using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToLevelUpBuyerProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassLevelUpBuyer LevelUpBuyer = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			LevelUpBuyer.OnBattlePassDescriptionArrived(battlePassDescription);
		}
	}
}
