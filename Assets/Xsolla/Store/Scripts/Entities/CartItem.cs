using System;

namespace Xsolla.Store
{
	[Serializable]
	public class CartItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		[Serializable]
		public class InventoryOptions
		{
			[Serializable]
			public class ConsumableOption
			{
				public int? usages_count;
			}
			public StoreItem.InventoryOptions.ConsumableOption consumable;
		}

		public string sku;
		public string name;
		public string type;
		public string virtual_item_type;
		public string description;
		public string image_url;
		public StoreItemAttribute[] attributes;
		public bool is_free;
		public bool is_bonus;
		public Group[] groups;
		public Price price;
		public VirtualPrice[] virtual_prices;
		public StoreItem.InventoryOptions inventory_options;
		public int quantity;
	}
}