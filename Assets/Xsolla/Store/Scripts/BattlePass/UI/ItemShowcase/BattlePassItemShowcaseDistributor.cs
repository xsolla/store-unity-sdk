using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemShowcaseDistributor : BaseBattlePassSelectedItemSubscriber
    {
		[SerializeField] private BaseBattlePassItemShowcase RegularItemShowcase;
		[SerializeField] private BaseBattlePassItemShowcase FinalItemShowcase;
		[SerializeField] private GameObject RegularItemShowcasePanel;
		[SerializeField] private GameObject FinalItemShowcasePanel;

		public override void OnItemSelected(ItemSelectedEventArgs eventArgs)
		{
			var isFinalItem = eventArgs.SelectedItemInfo.ItemDescription.IsFinal;

			if (isFinalItem)
			{
				SwapActive(FinalItemShowcasePanel, RegularItemShowcasePanel);
				FinalItemShowcase.OnItemSelected(eventArgs);
			}
			else
			{
				SwapActive(RegularItemShowcasePanel, FinalItemShowcasePanel);
				RegularItemShowcase.OnItemSelected(eventArgs);
			}
		}

		private void SwapActive(GameObject toActivate, GameObject toInactivate)
		{
			toActivate.SetActive(true);
			toInactivate.SetActive(false);
		}
	}
}
