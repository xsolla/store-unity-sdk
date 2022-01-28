using System;

namespace Xsolla.Store
{
	[Serializable]
	public class OrderItem
	{
		public string sku;
		public int quantity;
		public string is_free;
		public Price price;
	}
}