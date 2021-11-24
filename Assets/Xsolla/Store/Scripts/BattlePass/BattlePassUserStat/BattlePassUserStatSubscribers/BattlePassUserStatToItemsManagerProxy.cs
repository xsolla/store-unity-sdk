using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserStatToItemsManagerProxy : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private BattlePassItemsManager ItemsManager;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			ItemsManager.OnUserStatArrived(userStat);
		}
	}
}
