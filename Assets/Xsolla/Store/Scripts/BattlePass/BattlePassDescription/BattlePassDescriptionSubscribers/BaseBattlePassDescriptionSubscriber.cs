using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassDescriptionSubscriber : MonoBehaviour
    {
		public abstract void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription);
	}
}
