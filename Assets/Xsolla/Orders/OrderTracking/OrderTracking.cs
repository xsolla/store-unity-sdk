using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTracking : MonoSingleton<OrderTracking>
	{
		private readonly Dictionary<int, OrderTracker> _currentTrackers = new Dictionary<int, OrderTracker>();

		public bool IsTracking(int orderId)
		{
			return _currentTrackers.ContainsKey(orderId);
		}

		public bool AddOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (IsTracking(orderId))
				return false;

			var orderTrackingData = new OrderTrackingData(projectId,orderId,onSuccess,onError);
#if UNITY_WEBGL
			var tracker = (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
				? (OrderTracker)new OrderTrackerByPaystationCallbacks(orderTrackingData)
				: (OrderTracker)new OrderTrackerByShortPolling(orderTrackingData);
#else
			var tracker = new OrderTrackerByWebsockets(orderTrackingData);
#endif
			StartTracker(tracker);
			return true;
		}

		public bool AddVirtualCurrencyOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (IsTracking(orderId))
				return false;

			var orderTrackingData = new OrderTrackingData(projectId,orderId,onSuccess,onError);
#if UNITY_WEBGL
			var tracker = new OrderTrackerByShortPolling(orderTrackingData);
#else
			var tracker = new OrderTrackerByWebsockets(orderTrackingData);
#endif
			StartTracker(tracker);
			return true;
		}

		[Obsolete("Use AddOrderForShortPollingTracking instead")]
		public void AddOrderForTrackingUntilDone(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
			=> AddOrderForShortPollingTracking(projectId, orderId, onSuccess, onError);

		public bool AddOrderForShortPollingTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			if (IsTracking(orderId))
				return false;

			var orderTrackingData = new OrderTrackingData(projectId,orderId,onSuccess,onError);
			var tracker = new OrderTrackerByShortPolling(orderTrackingData);
			StartTracker(tracker);
			return true;
		}

		public bool RemoveOrderFromTracking(int orderId)
		{
			if (_currentTrackers.TryGetValue(orderId, out var tracker)) {
				tracker.Stop();
				_currentTrackers.Remove(orderId);
				return true;
			}

			return false;
		}

		public bool ReplaceTracker(OrderTracker oldTracker, OrderTracker newTracker)
		{
			if (RemoveOrderFromTracking(oldTracker.TrackingData.orderId)) {
				StartTracker(newTracker);
				return true;
			}

			return false;
		}

		private void StartTracker(OrderTracker tracker)
		{
			_currentTrackers.Add(tracker.TrackingData.orderId, tracker);
			tracker.Start();
		}
	}
}
