using System;
using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Store
{
	[Serializable]
	public class OrderStatus
	{
		static readonly Dictionary<string, OrderStatusType> StatusTypes =
			new Dictionary<string, OrderStatusType>()
			{
				{"new", OrderStatusType.New},
				{"paid", OrderStatusType.Paid}
			};
		
		public int order_id;
		public string status;
		public CartItems content;

		public OrderStatusType Status
		{
			get
			{
				if (StatusTypes.Keys.Contains(status))
				{
					return StatusTypes[status];
				}

				return OrderStatusType.Unknown;
			}
		}
	}
}