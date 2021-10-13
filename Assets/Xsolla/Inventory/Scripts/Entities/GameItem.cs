using System;

namespace Xsolla.Store
{
	[Serializable]
	public class GameItem
	{
		public string sku;
		public string name;
		public string type;
		public string unit_type;
		public string description;
		public string image_url;
		public StoreItem.Group[] groups;
		public StoreItemAttribute[] attributes;
		public Unit[] unit_items;

		[Serializable]
		public class Unit
		{
			public string sku;
			public string type;
			public bool is_free;
			public Price price;
			public VirtualPrice[] virtual_prices;
			public string drm_name;
			public string drm_sku;
			public bool has_keys;
			public bool is_pre_order;
			public string release_date;
		}
	}
}