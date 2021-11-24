using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemShowcaseFinal : BaseBattlePassItemShowcase
	{
		[SerializeField] private Color ActiveColor;
		[SerializeField] private Color TintedColor;
		[SerializeField] private BattlePassImageRatioFitter ItemImageRatioFitter;

		private const string ITEM_DESCRIPTION_TEMPLATE = "Reach level <color=#FF005B>{0}</color> to get this {1}";
		private const string COLLECT_INFO = "Available now!";
		private const string COLLECTED_INFO = "You own this item";
		private const string COLLECT_LOCKED_INFO = "Available in Battle Pass";
		private const string COLLECT_LOCKED_INFO_EXPIRED = "";

		protected override void AdditionalImageActions(ItemImageContainer imageContainer)
		{
			ItemImageRatioFitter.SetAspectRatio(imageContainer);
		}

		protected override void SetItemTextInfo(BattlePassItemDescription itemDescription)
		{
			base.ItemName.text = itemDescription.ItemCatalogModel.Name;
		}

		protected override void AdditionalActions(ItemSelectedEventArgs selectedEventArgs)
		{
			var selectedItemInfo = selectedEventArgs.SelectedItemInfo;
			var itemState = selectedItemInfo.ItemState;

			PostSetItemImage(itemState);
			PostSetItemDescription(itemState, selectedItemInfo.ItemDescription.ItemCatalogModel.Description, selectedItemInfo.ItemDescription.Tier);
			PostSetItemInfo(itemState);
		}

		private void PostSetItemImage(BattlePassItemState itemState)
		{
			switch (itemState)
			{
				case BattlePassItemState.Collect:
				case BattlePassItemState.Collected:
					base.ItemImage.color = ActiveColor;
					break;
				case BattlePassItemState.PremiumLocked:
				case BattlePassItemState.FutureLocked:
					base.ItemImage.color = TintedColor;
					break;
			}
		}

		private void PostSetItemDescription(BattlePassItemState itemState, string itemDescription, int itemLevel)
		{
			switch (itemState)
			{
				case BattlePassItemState.Collect:
				case BattlePassItemState.Collected:
					base.ItemDescription.text = itemDescription;
					break;
				case BattlePassItemState.PremiumLocked:
				case BattlePassItemState.FutureLocked:
					if (!base.IsBattlePassExpired)
						base.ItemDescription.text = string.Format(ITEM_DESCRIPTION_TEMPLATE, itemLevel, LowerDescription(itemDescription));
					else
						goto case BattlePassItemState.Collected;
					break;
			}
		}

		private string LowerDescription(string itemDescription)
		{
			if (string.IsNullOrEmpty(itemDescription))
				return string.Empty;

			if (itemDescription.Length < 2)
				return itemDescription;

			var firstChar = char.ToLowerInvariant(itemDescription[0]);
			var restChars = itemDescription.Substring(1);

			return string.Format("{0}{1}", firstChar, restChars);
		}

		private void PostSetItemInfo(BattlePassItemState itemState)
		{
			switch (itemState)
			{
				case BattlePassItemState.Collect:
					base.ItemInfo.text = COLLECT_INFO;
					break;
				case BattlePassItemState.Collected:
					base.ItemInfo.text = COLLECTED_INFO;
					break;
				case BattlePassItemState.PremiumLocked:
				case BattlePassItemState.FutureLocked:
					if (!base.IsBattlePassExpired)
						base.ItemInfo.text = COLLECT_LOCKED_INFO;
					else
						base.ItemInfo.text = COLLECT_LOCKED_INFO_EXPIRED;
					break;
			}
		}
	}
}
