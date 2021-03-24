using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemShowcaseDistributor : BaseBattlePassSelectedItemSubscriber
    {
		[SerializeField] private BaseBattlePassItemShowcase RegularItemShowcase = default;
		[SerializeField] private BaseBattlePassItemShowcase FinalItemShowcase = default;
		[SerializeField] private GameObject RegularItemShowcasePanel = default;
		[SerializeField] private GameObject FinalItemShowcasePanel = default;

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
