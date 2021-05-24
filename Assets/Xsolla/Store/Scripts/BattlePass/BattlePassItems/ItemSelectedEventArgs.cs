using System;

namespace Xsolla.Demo
{
	public class ItemSelectedEventArgs : EventArgs
    {
		public readonly BattlePassItemClickEventArgs SelectedItemInfo;
		public readonly BattlePassItemDescription[] AllItemsInCollectState;
		public readonly bool IsBattlePassExpired;

		public ItemSelectedEventArgs(BattlePassItemClickEventArgs selectedItemInfo, BattlePassItemDescription[] allItemsInCollectState, bool isBattlePassExpired)
		{
			SelectedItemInfo = selectedItemInfo;
			AllItemsInCollectState = allItemsInCollectState;
			IsBattlePassExpired = isBattlePassExpired;
		}
	}
}
