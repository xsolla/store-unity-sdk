using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassUserStatSubscriber : MonoBehaviour
    {
		public abstract void OnUserStatArrived(BattlePassUserStat userStat);
	}
}
