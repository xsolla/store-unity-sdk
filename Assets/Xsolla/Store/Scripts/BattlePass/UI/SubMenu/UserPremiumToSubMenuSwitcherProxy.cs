using UnityEngine;

namespace Xsolla.Demo
{
	public class UserPremiumToSubMenuSwitcherProxy : BaseBattlePassUserPremiumStatusSubscriber
	{
		[SerializeField] private BattlePassSubMenuSwitcher SubMenuSwitcher = default;

		public override void OnUserPremiumDefined(bool isPremiumUser)
		{
			SubMenuSwitcher.OnUserPremiumDefined(isPremiumUser);
		}
	}
}
