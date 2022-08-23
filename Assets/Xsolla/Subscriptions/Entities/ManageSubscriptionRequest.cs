using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class ManageSubscriptionRequest
	{
		public PaymentSettings settings;

		public ManageSubscriptionRequest(PaymentSettings settings)
		{
			this.settings = settings;
		}
	}
}