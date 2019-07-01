using System;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItem
	{
		public string sku;
		public string[] groups;
		public string name;
		public string type;
		public bool is_free;
		public string long_description;
		public string description;
		public string image_url;
		public Price[] prices;
	}
}