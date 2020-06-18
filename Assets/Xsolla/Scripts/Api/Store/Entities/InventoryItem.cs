using System;
using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Store
{
	[Serializable]
	public class InventoryItem
	{
		static readonly Dictionary<string, VirtualItemType> VirtualItemTypes =
			new Dictionary<string, VirtualItemType>()
			{
				{"consumable", VirtualItemType.Consumable},
				{"non_consumable", VirtualItemType.NonConsumable},
				{"non_renewing_subscription", VirtualItemType.NonRenewingSubscription}
			};

		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		public string sku;
		public string name;
		public string description;
		//public object attributes; Don't use it yet.
		public string type;
		public string virtual_item_type;
		public StoreItem.Group[] groups;
		public string image_url;
		public int? quantity;
		public int? remaining_uses;
		public string instance_id;

		public bool IsConsumable()
		{
			return VirtualItemType == VirtualItemType.Consumable;
		}
		
		public VirtualItemType VirtualItemType
		{
			get
			{
				if (virtual_item_type != null && VirtualItemTypes.Keys.Contains(virtual_item_type))
				{
					return VirtualItemTypes[virtual_item_type];
				}

				return VirtualItemType.None;
			}
		}
	}
}

