using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	internal static class OrderTrackingService
	{
		private static readonly Dictionary<int, OrderTracker> Trackers = new Dictionary<int, OrderTracker>();

		public static void AddOrderForTracking(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError)
		{
			var tracker = CreateTracker(orderId, isUserInvolvedToPayment, onSuccess, onError);
			if (tracker != null)
				StartTracker(tracker);
		}

		private static OrderTracker CreateTracker(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError)
		{
			if (Trackers.ContainsKey(orderId))
				return null;

			var trackingData = new OrderTrackingData(orderId, onSuccess, onError);
#if UNITY_WEBGL
			if (!isUserInvolvedToPayment)
				return new OrderTrackerByShortPolling(trackingData);

			return !UnityEngine.Application.isEditor && XsollaSettings.InAppBrowserEnabled
				? new OrderTrackerByPaystationCallbacks(trackingData)
				: new OrderTrackerByShortPolling(trackingData) as OrderTracker;
#else
			return new OrderTrackerByWebsockets(trackingData);
#endif
		}

		public static bool RemoveOrderFromTracking(int orderId)
		{
			if (!Trackers.TryGetValue(orderId, out var tracker))
				return false;

			tracker.Stop();
			Trackers.Remove(orderId);
			return true;
		}

		public static bool ReplaceTracker(OrderTracker oldTracker, OrderTracker newTracker)
		{
			if (!RemoveOrderFromTracking(oldTracker.TrackingData.orderId))
				return false;

			StartTracker(newTracker);
			return true;
		}

		private static void StartTracker(OrderTracker tracker)
		{
			XDebug.Log($"Order tracker started: {tracker.GetType().Name}");
			Trackers.Add(tracker.TrackingData.orderId, tracker);
			tracker.Start();
		}
	}
}