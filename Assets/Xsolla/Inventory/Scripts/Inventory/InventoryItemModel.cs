using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class InventoryItemModel : ItemModel
	{
		public override bool IsVirtualCurrency() { return false; }
		public override bool IsSubscription() { return false; }
		public override bool IsBundle() { return false; }

		public string InstanceId;
		public uint? RemainingUses;
	}
}
