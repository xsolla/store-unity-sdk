using UnityEngine;

namespace Xsolla.Demo
{
	public class UserStatToLevelUpBuyerProxy : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private BattlePassLevelUpBuyer LevelUpBuyer = default;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			LevelUpBuyer.OnUserStatArrived(userStat);
		}
	}
}
