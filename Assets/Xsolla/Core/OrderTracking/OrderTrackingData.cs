using System;

namespace Xsolla.Core
{
	public class OrderTrackingData
	{
		public readonly int orderId;
		public readonly Action successCallback;
		public readonly Action<Error> errorCallback;
		public readonly SdkType sdkType;

		public OrderTrackingData(int orderId, Action successCallback, Action<Error> errorCallback, SdkType sdkType)
		{
			this.orderId = orderId;
			this.successCallback = successCallback;
			this.errorCallback = errorCallback;
			this.sdkType = sdkType;
		}
	}
}