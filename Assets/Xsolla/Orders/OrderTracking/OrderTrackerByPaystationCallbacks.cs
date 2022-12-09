using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackerByPaystationCallbacks : OrderTracker
	{
		public override void Start()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate += CheckOrderStatus;
			XsollaWebCallbacks.Instance.OnPaymentCancel += RemoveSelfFromTracking;
		}

		public override void Stop()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate -= CheckOrderStatus;
			XsollaWebCallbacks.Instance.OnPaymentCancel -= RemoveSelfFromTracking;
#if UNITY_WEBGL
			BrowserHelper.ClosePaystationWidget();
#endif
		}

		private void CheckOrderStatus()
		{
			CheckOrderStatus(
				onDone: () =>
				{
					trackingData.successCallback?.Invoke();
					RemoveSelfFromTracking();
				},
				onCancel: RemoveSelfFromTracking,
				onError: error =>
				{
					trackingData.errorCallback?.Invoke(error);
					RemoveSelfFromTracking();
				}
			);
		}

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}