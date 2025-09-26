using System;
using System.Collections.Generic;

namespace Xsolla.Subscriptions
{
	[Serializable]
	internal class BuySubscriptionRequest
	{
		public string plan_external_id;
		public PaymentSettings settings;
		public Dictionary<string, object> custom_parameters;
	}
}