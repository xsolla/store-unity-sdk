using System;

namespace Xsolla.Store
{
	[Serializable]
	public class PromocodeReward
	{
		[Serializable]
		public class UnitItem
		{
			public string sku;
			public string type;
			public string name;
			public string drm_name;
			public string drm_sku;
		}

		[Serializable]
		public class RewardItem
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public UnitItem[] unit_items;
		}

		[Serializable]
		public class BonusItem
		{
			public RewardItem item;
			public int quantity;
		}
		
		[Serializable]
		public class Discount
		{
			public string percent;
		}

		public BonusItem[] bonus;
		public Discount discount;
		public bool is_selectable;
	}
}