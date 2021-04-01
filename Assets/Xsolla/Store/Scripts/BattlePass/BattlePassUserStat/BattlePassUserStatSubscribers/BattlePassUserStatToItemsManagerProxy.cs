using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserStatToItemsManagerProxy : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private BattlePassItemsManager ItemsManager = default;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			ItemsManager.OnUserStatArrived(userStat);
		}
	}
}
