using System;
using Xsolla.Core;

namespace Xsolla.Orders
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