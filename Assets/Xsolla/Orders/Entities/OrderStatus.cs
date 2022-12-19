using System;

namespace Xsolla.Orders
{
	[Serializable]
	public class OrderStatus
	{
		public int order_id;
		
		public string status;
		
		public OrderContent content;
	}
}