using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class RenewalSubscriptionRequest
	{
		public PaymentSettings settings;

		public RenewalSubscriptionRequest(bool sandbox)
		{
			settings = new PaymentSettings(sandbox);
		}
	}
}