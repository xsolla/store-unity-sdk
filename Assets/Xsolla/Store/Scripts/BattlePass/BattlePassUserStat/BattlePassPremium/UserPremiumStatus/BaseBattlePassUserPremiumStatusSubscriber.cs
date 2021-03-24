using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassUserPremiumStatusSubscriber : MonoBehaviour
	{
		public abstract void OnUserPremiumDefined(bool isPremiumUser);
	}
}
