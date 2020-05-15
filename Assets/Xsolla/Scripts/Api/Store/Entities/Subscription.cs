using System;
using Newtonsoft.Json;

namespace Xsolla.Store
{
	[Serializable]
	public class Subscription
	{
		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public string status;
		public long expired_at;
		
		[JsonProperty("class")]
		public string subscription_class;
	}
}