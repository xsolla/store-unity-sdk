using System;

namespace Xsolla.Store
{
	[Serializable]
	public class InventoryItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		public string sku;
		public string name;
		public string description;
		public string[] attributes;
		public string type;
		public StoreItem.Group[] groups;
		public string image_url;
		public int quantity;
		public int? remaining_uses;
		public string instance_id;

		public bool IsConsumable()
		{
			return remaining_uses != null;
		}
	}
}

