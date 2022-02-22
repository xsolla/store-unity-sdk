using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackerByPaystationCallbacks : OrderTracker
	{
		public override void Start()
		{
#if UNITY_WEBGL
			XsollaWebCallbacks.PaymentStatusUpdate += CheckOrderStatus;
			XsollaWebCallbacks.PaymentCancel += RemoveSelfFromTracking;
#endif
		}

		public override void Stop()
		{
#if UNITY_WEBGL
			XsollaWebCallbacks.PaymentStatusUpdate -= CheckOrderStatus;
			XsollaWebCallbacks.PaymentCancel -= RemoveSelfFromTracking;
			BrowserHelper.ClosePaystationWidget();
#endif
		}

		private void CheckOrderStatus()
		{
			CheckOrderStatus(
				onDone: () =>
				{
					trackingData.SuccessCallback?.Invoke();
					RemoveSelfFromTracking();
				},
				onCancel: RemoveSelfFromTracking,
				onError: error =>
				{
					trackingData.ErrorCallback?.Invoke(error);
					RemoveSelfFromTracking();
				}
			);
		}

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}
