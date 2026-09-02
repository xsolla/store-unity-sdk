using System;
using System.Collections.Generic;

namespace Xsolla.Subscriptions
{
	[Serializable]
	internal class BuySubscriptionRequest
	{
		public string plan_external_id;
		public int? product_id;
		public string[] available_plans;
		public PaymentSettings settings;
		public Dictionary<string, object> custom_parameters;
	}
}