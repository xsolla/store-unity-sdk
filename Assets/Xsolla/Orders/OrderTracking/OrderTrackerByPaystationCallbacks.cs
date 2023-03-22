namespace Xsolla.Orders
{
	public class OrderTrackerByPaystationCallbacks : OrderTracker
	{
#if UNITY_WEBGL
		private bool isCancelByUser;
#endif

		public override void Start()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate += HandleStatusUpdate;
			XsollaWebCallbacks.Instance.OnPaymentCancel += HandlePaymentCancel;
		}

		public override void Stop()
		{
			XsollaWebCallbacks.Instance.OnPaymentStatusUpdate -= HandleStatusUpdate;
			XsollaWebCallbacks.Instance.OnPaymentCancel -= HandlePaymentCancel;
#if UNITY_WEBGL
			Xsolla.Core.BrowserHelper.ClosePaystationWidget(isCancelByUser);
#endif
		}

		private void HandleStatusUpdate()
		{
			CheckOrderStatus(
				onDone: () =>
				{
					TrackingData?.successCallback?.Invoke();
					RemoveSelfFromTracking();
				},
				onCancel: RemoveSelfFromTracking,
				onError: error =>
				{
					TrackingData?.errorCallback?.Invoke(error);
					RemoveSelfFromTracking();
				}
			);
		}

		private void HandlePaymentCancel()
		{
#if UNITY_WEBGL
			isCancelByUser = true;
#endif
			RemoveSelfFromTracking();
		}

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData) : base(trackingData) { }
	}
}