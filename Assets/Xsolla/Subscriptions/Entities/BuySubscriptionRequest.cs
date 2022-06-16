using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class BuySubscriptionRequest
	{
		public string plan_external_id;
		public PaymentSettings settings;

		public BuySubscriptionRequest(string planExternalId, bool sandbox)
		{
			plan_external_id = planExternalId;
			settings = new PaymentSettings(sandbox);
		}
	}
}