using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTracking : MonoSingleton<OrderTracking>
	{
		private readonly Dictionary<int, OrderTracker> trackers = new Dictionary<int, OrderTracker>();

		public void AddOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (trackers.ContainsKey(orderId))
				return;

			var orderTrackingData = CreateOrderTrackingData(projectId, orderId, onSuccess, onError);
			var tracker = CreateTracker(orderTrackingData);
			StartTracker(tracker);
		}

		public void AddOrderForTrackingUntilDone(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (trackers.ContainsKey(orderId))
				return;

			var orderTrackingData = CreateOrderTrackingData(projectId, orderId, onSuccess, onError);
			var tracker = new OrderTrackerByShortPolling(orderTrackingData, this);
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
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
				return new OrderTrackerByPaystationCallbacks(trackingData, this);
#endif

			return new OrderTrackerByWebsockets(trackingData, this);
		}
	}
}
