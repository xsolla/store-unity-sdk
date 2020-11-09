using System;

namespace Xsolla.Store
{
	[Serializable]
	public class CouponRedeemedItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		[Serializable]
		public class Attribute
		{
			public int stack_size;
			public bool licensed;
		}

		public string sku;
		public Group[] groups;
		public string name;
		public Attribute attributes;
		public string type;
		public string description;
		public string image_url;
		public int quantity;
		public bool is_free;
	}
}