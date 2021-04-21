using System;

namespace Xsolla.Store
{
	[Serializable]
	public class BundleItem
	{
		public string sku;
		public string name;
		public Group[] groups;
		public string description;
		public StoreItemAttribute[] attributes;
		public string type;
		public string bundle_type;
		public string image_url;
		public bool is_free;
		public Price price;
		public Price total_content_price;
		public VirtualPrice[] virtual_prices;
		public Content[] content;
		
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		
		[Serializable]
		public class Content
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public int quantity;
			public Price price;
			public VirtualPrice[] virtual_prices;
		}
	}
}
