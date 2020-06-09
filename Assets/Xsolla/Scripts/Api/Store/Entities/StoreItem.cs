using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItem
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
		[Serializable]
		public class InventoryOptions
		{
			[Serializable]
			public class ConsumableOption
			{
				public int? usages_count;
			}
			[Serializable]
			public class ExpirationPeriod
			{
				public string type;
				public int value;
			}
			public ConsumableOption consumable;
			public ExpirationPeriod expiration_period;
		}
		
		public string sku;
		public string name;
		public Group[] groups;
		public string[] attributes;
		public string type;
		public string virtual_item_type;
		public string description;
		public string image_url;
		public bool is_free;
		public Price price;
		public VirtualPrice[] virtual_prices;
		public InventoryOptions inventory_options;
		
		public StoreItem DeepClone()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<StoreItem>(json);
		}

		public bool IsConsumable()
		{
			return VirtualItemType == VirtualItemType.Consumable;
		}

		public bool IsSubscription()
		{
			return VirtualItemType == VirtualItemType.NonRenewingSubscription;
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
