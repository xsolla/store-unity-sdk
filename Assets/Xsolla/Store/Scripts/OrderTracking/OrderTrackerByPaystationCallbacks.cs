using Xsolla.Core;

namespace Xsolla.Store
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
					if (trackingData != null && trackingData.SuccessCallback != null)
						trackingData.SuccessCallback.Invoke();
					RemoveSelfFromTracking();
				},
				onCancel: RemoveSelfFromTracking,
				onError: error =>
				{
					if (trackingData != null && trackingData.ErrorCallback != null)
						trackingData.ErrorCallback.Invoke(error);
					RemoveSelfFromTracking();
				}
			);
		}

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}
