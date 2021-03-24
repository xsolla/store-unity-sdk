using UnityEngine;

namespace Xsolla.Demo
{
	public class ItemSelectedToSubMenuSwitcherProxy : BaseBattlePassSelectedItemSubscriber
    {
		[SerializeField] private BattlePassSubMenuSwitcher SubMenuSwitcher = default;

		public override void OnItemSelected(ItemSelectedEventArgs eventArgs)
		{
			SubMenuSwitcher.OnItemSelectedArrived(eventArgs);
		}
	}
}
