using System;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		[Serializable]
		public class VirtualPrice
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public string amount;
			public string amount_without_discount;
			public bool is_default;
		}
		public string sku;
		public string name;
		public string[] attributes;
		public string[] media_list;
		public string type;
		public Price price;
		public StoreItem.VirtualPrice[] virtual_prices;
		public StoreItem.Group[] groups;
		public string long_description;
		public string description;
		public string image_url;
		public string promotion;
		public bool is_free;
		public int order;
		public string purchase_limit;
	}
}
