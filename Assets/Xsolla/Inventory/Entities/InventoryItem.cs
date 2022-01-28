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
		private const string VIRTUAL_CURRENCY_TYPE = "virtual_currency";

		public string sku;
		public string name;
		public string description;
		//Public object attributes; Don't use it yet.
		public string type;
		public string virtual_item_type;
		public StoreItemAttribute[] attributes;
		public StoreItemGroup[] groups;
		public string image_url;
		public uint? quantity;
		public uint? remaining_uses;
		public string instance_id;

		public bool IsConsumable()
		{
			return VirtualItemType == VirtualItemType.Consumable;
		}

		public bool IsVirtualCurrency()
		{
			return !string.IsNullOrEmpty(type) && type.Equals(VIRTUAL_CURRENCY_TYPE);
		}

		public bool IsSubscription()
		{
			return VirtualItemType == VirtualItemType.NonRenewingSubscription;
		}
		
		public VirtualItemType VirtualItemType
		{
			get
			{
				if (!string.IsNullOrEmpty(virtual_item_type) && VirtualItemTypes.Keys.Contains(virtual_item_type))
				{
					return VirtualItemTypes[virtual_item_type];
				}

				return VirtualItemType.None;
			}
		}
	}
}

