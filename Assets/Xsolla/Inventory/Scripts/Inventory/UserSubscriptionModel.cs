using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class UserSubscriptionModel : ItemModel
	{
		public enum SubscriptionStatusType
		{
			None,
			Active,
			Expired
		}
	
		public override bool IsVirtualCurrency() { return false; }
		public override bool IsSubscription() { return true; }
		public override bool IsBundle() { return false; }

		public SubscriptionStatusType Status { get; set; }
		public DateTime? Expired { get; set; }
	}
}
