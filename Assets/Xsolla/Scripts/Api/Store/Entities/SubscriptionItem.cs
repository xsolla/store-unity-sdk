using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Xsolla.Store
{
	[Serializable]
	public class SubscriptionItem
	{
		static readonly Dictionary<string, SubscriptionStatusType> StatusTypes =
			new Dictionary<string, SubscriptionStatusType>()
			{
				{"none", SubscriptionStatusType.None},
				{"active", SubscriptionStatusType.Active},
				{"expired", SubscriptionStatusType.Expired}
			};

		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public string status;
		public long? expired_at;

		[JsonProperty("class")]
		public string subscription_class;
		
		public SubscriptionStatusType Status
		{
			get
			{
				if (StatusTypes.Keys.Contains(status))
				{
					return StatusTypes[status];
				}

				return SubscriptionStatusType.None;
			}
		}
	}
}