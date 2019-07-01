using System;

namespace Xsolla.Store
{
	[Serializable]
	public class CartItem
	{
		public string sku;
		public string[] groups;
		public string name;
		public string type;
		public bool is_free;
		public int order;
		public string long_description;
		public string description;
		public string image_url;
		public CartPrice price;
		public int quantity;
	}
}