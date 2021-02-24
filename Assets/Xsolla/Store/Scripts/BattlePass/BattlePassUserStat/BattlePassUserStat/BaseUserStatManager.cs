using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseUserStatManager : MonoBehaviour
    {
		public event Action<BattlePassUserStat> UserStatArrived;

		public abstract void GetUserStat(BattlePassDescription battlePassDescription);

		protected void RaiseUserStatArrived(BattlePassUserStat userStat)
		{
			UserStatArrived?.Invoke(userStat);
		}
	}
}
