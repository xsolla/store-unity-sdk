using System;
using System.Collections.Generic;

namespace Xsolla.Subscriptions
{
	[Serializable]
	internal class ManageSubscriptionRequest
	{
		public int? product_id;
		public string[] available_plans;
		public PaymentSettings settings;
		public Dictionary<string, object> custom_parameters;
		public string steam_user_id;
	}
}