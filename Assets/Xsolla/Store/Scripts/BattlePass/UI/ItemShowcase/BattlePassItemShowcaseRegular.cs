using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassItemShowcaseRegular : BaseBattlePassItemShowcase
    {
		[SerializeField] private Image Background = default;
		[SerializeField] private GameObject CollectOne = default;
		[SerializeField] private GameObject CollectAll = default;

		private const string ITEM_QUANTITY_TEMPLATE = "Quantity: {0} piece{1}";
		private const string ITEM_FUTURE_LOCKED_TEMPLATE = "Reach level <color=#FF005B>{0}</color> to get this reward";

		protected override void SetItemTextInfo(BattlePassItemDescription itemDescription)
		{
			ItemName.text = itemDescription.ItemCatalogModel.Name;
			ItemDescription.text = itemDescription.ItemCatalogModel.Description;
			ItemInfo.text = string.Format(ITEM_QUANTITY_TEMPLATE, itemDescription.Quantity, itemDescription.Quantity == 1 ? string.Empty : "s");
		}

		protected override void AdditionalImageActions(ItemImageContainer imageContainer)
		{
			StartCoroutine(base.SetItemImage(Background, imageContainer));
		}

		protected override void AdditionalActions(ItemSelectedEventArgs selectedEventArgs)
		{
			//Additional state actions
			switch (selectedEventArgs.SelectedItemInfo.ItemState)
			{
				case BattlePassItemState.Collect:
					if (selectedEventArgs.AllItemsInCollectState.Length > 1)
						SwapActive(CollectAll, CollectOne);
					else
						SwapActive(CollectOne, CollectAll);
					break;
				case BattlePassItemState.FutureLocked:
					ItemInfo.text = string.Format(ITEM_FUTURE_LOCKED_TEMPLATE, selectedEventArgs.SelectedItemInfo.ItemDescription.Tier);
					break;
				default:
					break;
			}
		}

		private void SwapActive(GameObject toActivate, GameObject toInactivate)
		{
			toActivate.SetActive(true);
			toInactivate.SetActive(false);
		}
	}
}
