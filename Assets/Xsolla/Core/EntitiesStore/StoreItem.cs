using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Xsolla.Core
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

				public TimeSpan ToTimeSpan()
				{
					var dt = DateTime.Now;
					switch (type)
					{
						case "minute":
							dt = dt.AddMinutes(value);
							break;
						case "hour":
							dt = dt.AddHours(value);
							break;
						case "day":
							dt = dt.AddDays(value);
							break;
						case "week":
							dt = dt.AddDays(7 * value);
							break;
						case "month":
							dt = dt.AddMonths(value);
							break;
						case "year":
							dt = dt.AddYears(value);
							break;
					}
					return dt - DateTime.Now;
				}

				public override string ToString()
				{
					var result = $"{value} {type}";
					if (value > 1)
						result += "s";
					return result;
				}
			}
			public ConsumableOption consumable;
			public ExpirationPeriod expiration_period;
		}
		
		public string sku;
		public string name;
		public StoreItemGroup[] groups;
		public StoreItemAttribute[] attributes;
		public string type;
		public string virtual_item_type;
		public string description;
		public string long_description;
		public string image_url;
		public bool is_free;
		public Price price;
		public VirtualPrice[] virtual_prices;
		public InventoryOptions inventory_options;
		public int order;
		public MediaListItem[] media_list;
		public StoreItemPromotion[] promotions;

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
