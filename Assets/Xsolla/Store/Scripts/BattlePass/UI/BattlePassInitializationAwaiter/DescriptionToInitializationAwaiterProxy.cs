using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToInitializationAwaiterProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassInitializationAwaiter Awaiter;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			Awaiter.OnBattlePassDescriptionArrived(battlePassDescription);
		}
	}
}
