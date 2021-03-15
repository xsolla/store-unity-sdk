using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToLevelUpControllerProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassLevelUpController LevelUpShower = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			LevelUpShower.SetBattlePassLevels(battlePassDescription.Levels.Length);
		}
	}
}
