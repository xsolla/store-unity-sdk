using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class ManageSubscriptionRequest
	{
		public PaymentSettings settings;

		public ManageSubscriptionRequest(bool sandbox)
		{
			settings = new PaymentSettings(sandbox);
		}
	}
}