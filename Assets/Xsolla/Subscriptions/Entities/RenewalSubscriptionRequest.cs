using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class RenewalSubscriptionRequest
	{
		public PaymentSettings settings;

		public RenewalSubscriptionRequest(PaymentSettings settings)
		{
			this.settings = settings;
		}
	}
}