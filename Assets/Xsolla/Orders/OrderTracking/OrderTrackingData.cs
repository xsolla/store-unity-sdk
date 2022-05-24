using System;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackingData
	{
		public readonly string projectId;
		public readonly int orderId;
		public readonly Action successCallback;
		public readonly Action<Error> errorCallback;
		
		public OrderTrackingData(string projectId, int orderId, Action successCallback, Action<Error> errorCallback)
		{
			this.projectId = projectId;
			this.orderId = orderId;
			this.successCallback = successCallback;
			this.errorCallback = errorCallback;
		}
	}
}
