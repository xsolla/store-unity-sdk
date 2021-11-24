using UnityEngine;

namespace Xsolla.Demo
{
	public class UserPremiumStatusToItemsManagerProxy : BaseBattlePassUserPremiumStatusSubscriber
    {
		[SerializeField] private BattlePassItemsManager ItemsManager;

		public override void OnUserPremiumDefined(bool isPremiumUser)
		{
			ItemsManager.OnUserPremiumDefined(isPremiumUser);
		}
	}
}
