using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Store
{
	public class OrderTracking : MonoSingleton<OrderTracking>
	{
		private readonly Dictionary<int, OrderTracker> trackers = new Dictionary<int, OrderTracker>();

		public void AddOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			var data = CreateOrderTrackingData(projectId, orderId, onSuccess, onError);
			if (data == null)
				return;

			var tracker = CreateTracker(data);
			StartTracker(tracker);
		}

		public void AddOrderForTrackingUntilDone(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			var data = CreateOrderTrackingData(projectId, orderId, onSuccess, onError);
			if (data == null)
				return;

			var tracker = new OrderTrackerByShortPolling(data, this);
			StartTracker(tracker);
		}

		public void RemoveOrderFromTracking(int orderId)
		{
			if (!trackers.TryGetValue(orderId, out var tracker))
				return;

			tracker.Stop();
			trackers.Remove(orderId);
		}

		private void StartTracker(OrderTracker tracker)
		{
			trackers.Add(tracker.trackingData.OrderId, tracker);
			tracker.Start();
		}

		public void ReplaceTracker(OrderTracker oldTracker, OrderTracker newTracker)
		{
			RemoveOrderFromTracking(oldTracker.trackingData.OrderId);
			StartTracker(newTracker);
		}

		private OrderTrackingData CreateOrderTrackingData(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (trackers.ContainsKey(orderId))
				return null;

			return new OrderTrackingData{
				ProjectId = projectId,
				OrderId = orderId,
				SuccessCallback = onSuccess,
				ErrorCallback = onError
			};
		}

		private OrderTracker CreateTracker(OrderTrackingData trackingData)
		{
#if UNITY_WEBGL
			if (XsollaSettings.InAppBrowserEnabled)
				return new OrderTrackerByPaystationCallbacks(trackingData, this);
#endif

			return new OrderTrackerByWebsockets(trackingData, this);
		}
	}
}