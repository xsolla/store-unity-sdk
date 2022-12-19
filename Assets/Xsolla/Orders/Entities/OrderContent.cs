using System;
using Xsolla.Core;

namespace Xsolla.Orders
{
	[Serializable]
	public class OrderContent
	{
		public Price price;
		public VirtualPrice virtual_price;
		public string is_free;
		public OrderItem[] items;
	}
}