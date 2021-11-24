using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserPremiumStatusDistributor : MonoBehaviour
    {
		[SerializeField] private BattlePassUserPremiumStatusProvider PremiumProvider;
		[SerializeField] private BaseBattlePassUserPremiumStatusSubscriber[] UserPremiumSubscribers;

		private void Awake()
		{
			PremiumProvider.UserPremiumDefined += OnUserPremiumDefined;
		}

		private void OnUserPremiumDefined(bool isPremiumUser)
		{
			foreach (var subscriber in UserPremiumSubscribers)
				subscriber.OnUserPremiumDefined(isPremiumUser);
		}
	}
}
