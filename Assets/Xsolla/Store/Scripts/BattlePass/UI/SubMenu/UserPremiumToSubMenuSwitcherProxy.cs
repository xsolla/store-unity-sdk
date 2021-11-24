using UnityEngine;

namespace Xsolla.Demo
{
	public class UserPremiumToSubMenuSwitcherProxy : BaseBattlePassUserPremiumStatusSubscriber
	{
		[SerializeField] private BattlePassSubMenuSwitcher SubMenuSwitcher;

		public override void OnUserPremiumDefined(bool isPremiumUser)
		{
			SubMenuSwitcher.OnUserPremiumDefined(isPremiumUser);
		}
	}
}
