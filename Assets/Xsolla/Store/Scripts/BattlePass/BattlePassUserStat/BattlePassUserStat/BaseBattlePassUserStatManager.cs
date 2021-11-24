using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassUserStatManager : MonoBehaviour
    {
		public event Action<BattlePassUserStat> UserStatArrived;

		protected void RaiseUserStatArrived(BattlePassUserStat userStat)
		{
			if (UserStatArrived != null)
				UserStatArrived.Invoke(userStat);
		}

		public abstract void GetUserStat(BattlePassDescription battlePassDescription);

		public abstract void AddLevel(int levelsToAdd);
		public abstract void AddExp(int expToAdd);
		public abstract void AddObtainedItems(int[] freeItemsToAdd = null, int[] premiumItemsToAdd = null);
	}
}
