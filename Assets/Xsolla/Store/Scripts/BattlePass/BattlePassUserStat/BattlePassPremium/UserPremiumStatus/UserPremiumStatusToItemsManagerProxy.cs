using UnityEngine;

namespace Xsolla.Demo
{
	public class UserPremiumStatusToItemsManagerProxy : BaseBattlePassUserPremiumStatusSubscriber
    {
		[SerializeField] private BattlePassItemsManager ItemsManager = default;

		public override void OnUserPremiumDefined(bool isPremiumUser)
		{
			ItemsManager.OnUserPremiumDefined(isPremiumUser);
		}
	}
}
