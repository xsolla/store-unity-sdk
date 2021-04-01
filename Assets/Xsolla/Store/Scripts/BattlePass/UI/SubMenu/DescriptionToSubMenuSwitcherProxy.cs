using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToSubMenuSwitcherProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassSubMenuSwitcher SubMenuSwitcher = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			SubMenuSwitcher.OnDescriptionArrived(battlePassDescription);
		}
	}
}
