using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToUserStatProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BaseBattlePassUserStatManager UserStatManager = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			UserStatManager.GetUserStat(battlePassDescription);
		}
	}
}
