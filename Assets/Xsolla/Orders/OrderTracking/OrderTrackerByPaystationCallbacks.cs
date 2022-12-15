using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackerByPaystationCallbacks : OrderTracker
	{
		public override void Start()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate += HandleStatusUpdate;
			XsollaWebCallbacks.Instance.OnPaymentCancel += RemoveSelfFromTracking;
		}

		public override void Stop()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate -= HandleStatusUpdate;
			XsollaWebCallbacks.Instance.OnPaymentCancel -= RemoveSelfFromTracking;
#if UNITY_WEBGL
			BrowserHelper.ClosePaystationWidget();
#endif
		}

		private void HandleStatusUpdate()
		{
			base.CheckOrderStatus(
				onDone: () =>
				{
					base.TrackingData?.successCallback?.Invoke();
					base.RemoveSelfFromTracking();
				},
				onCancel: base.RemoveSelfFromTracking,
				onError: error =>
				{
					base.TrackingData?.errorCallback?.Invoke(error);
					base.RemoveSelfFromTracking();
				}
			);
		}

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData) : base(trackingData)
		{
		}
	}
}