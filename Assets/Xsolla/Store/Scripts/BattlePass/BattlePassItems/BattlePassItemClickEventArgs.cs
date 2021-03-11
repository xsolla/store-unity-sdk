using System;

namespace Xsolla.Demo
{
	public class BattlePassItemClickEventArgs : EventArgs
    {
		public readonly BattlePassItemDescription ItemDescription;
		public readonly BattlePassItemState ItemState;
		public readonly ItemImageContainer ItemImage;

		public BattlePassItemClickEventArgs(BattlePassItemDescription itemDescription, BattlePassItemState itemState, ItemImageContainer itemImage)
		{
			ItemDescription = itemDescription;
			ItemState = itemState;
			ItemImage = itemImage;
		}
	}
}
