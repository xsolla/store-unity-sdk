using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class SubscriptionItems
	{
		public SubscriptionItem[] items;
		public bool has_more;
	}
}