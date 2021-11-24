using UnityEngine;

namespace Xsolla.Demo
{
	public class UserPremiumToInitializationAwaiterProxy : BaseBattlePassUserPremiumStatusSubscriber
	{
		[SerializeField] private BattlePassInitializationAwaiter Awaiter;

		public override void OnUserPremiumDefined(bool isPremiumUser)
		{
			Awaiter.OnUserPremiumStatusDefined();
		}
	}
}
