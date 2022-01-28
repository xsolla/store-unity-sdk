using System;

namespace Xsolla.Store
{
	[Serializable]
	public class OrderStatus
	{
		public int order_id;
		
		public string status;
		
		public OrderContent content;
	}
}