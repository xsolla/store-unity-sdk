using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseUserStatSubscriber : MonoBehaviour
    {
		public abstract void OnUserStatArrived(BattlePassUserStat userStat);
	}
}
