using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class VirtualCurrencyBalanceModel : ItemModel
	{
		public override bool IsVirtualCurrency() { return true; }
		public override bool IsSubscription() { return false; }
		public override bool IsBundle() { return false; }

		public uint Amount { get; set; }
	}
}
