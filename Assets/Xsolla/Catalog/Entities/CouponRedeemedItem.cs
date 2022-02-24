using System;

namespace Xsolla.Catalog
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

		public string sku;
		public Group[] groups;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public int quantity;
		public bool is_free;
		// public object attributes; Don't use it yet.
	}
}
