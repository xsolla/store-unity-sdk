using System;
using System.Collections.Generic;

namespace Xsolla.Subscriptions
{
	[Serializable]
	internal class RenewalSubscriptionRequest
	{
		public PaymentSettings settings;
		public Dictionary<string, object> custom_parameters;
		public string steam_user_id;
	}
}