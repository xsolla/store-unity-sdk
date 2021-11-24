using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToLevelUpDataProviderProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassLevelUpDataProvider LevelUpProvider;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			LevelUpProvider.OnBattlePassDescriptionArrived(battlePassDescription.Name);
		}
	}
}
