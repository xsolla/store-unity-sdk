using UnityEngine;

namespace Xsolla.Demo
{
	public class UserStatToLevelUpControllerProxy : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private BattlePassLevelUpController LevelUpShower = default;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			LevelUpShower.SetUserLevel(userStat.Level);
		}
	}
}
