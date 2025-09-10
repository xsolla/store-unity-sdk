using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	public static class OrderTrackingService
	{
		private static readonly Dictionary<int, OrderTracker> Trackers = new Dictionary<int, OrderTracker>();

		/// <summary>
		/// Starts status tracking for the specified order. The tracking mechanism varies based on the build platform.
		/// </summary>
		/// <param name="orderId">Order ID.</param>
		/// <param name="isUserInvolvedToPayment">Whether to use platform-specific methods for tracking, such as Web Sockets or Pay Station callbacks.</param>
		/// <param name="onSuccess">Callback, triggered when the order status is changed to `done`</param>
		/// <param name="onError">Callback, triggered when an error occurs during the order tracking.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void AddOrderForTracking(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError, SdkType sdkType = SdkType.Store)
		{
			var tracker = CreateTracker(orderId, isUserInvolvedToPayment, onSuccess, onError, sdkType);
			if (tracker != null)
				StartTracker(tracker);
		}

		private static OrderTracker CreateTracker(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError, SdkType sdkType)
		{
			if (Trackers.ContainsKey(orderId))
				return null;

			var trackingData = new OrderTrackingData(orderId, onSuccess, onError, sdkType);

			if (!isUserInvolvedToPayment)
				return new OrderTrackerByShortPolling(trackingData);

#if UNITY_WEBGL && !UNITY_EDITOR
			var isSafariBrowser = WebHelper.IsBrowserSafari();
			return !UnityEngine.Application.isEditor && XsollaSettings.InAppBrowserEnabled && !isSafariBrowser
				? new OrderTrackerByPaystationCallbacks(trackingData)
				: new OrderTrackerByShortPolling(trackingData);
#else
			return new OrderTrackerByCentrifugo(trackingData);
#endif
		}

		/// <summary>
		/// Stops status tracking for all orders.
		/// </summary>
		public static void RemoveAllOrdersFromTracking()
		{
			foreach (var tracker in Trackers.Values)
			{
				RemoveOrderFromTracking(tracker.TrackingData.orderId);
			}
		}

		/// <summary>
		/// Stops status tracking for specified order.
		/// </summary>
		/// <param name="orderId">Order ID.</param>
		public static void RemoveOrderFromTracking(int orderId)
		{
			if (!Trackers.TryGetValue(orderId, out var tracker))
				return;

			XDebug.Log($"Order tracker '{tracker.GetType().Name}' stopped for order: {orderId}");
			tracker.Stop();
			Trackers.Remove(orderId);
		}

		public static void ReplaceTracker(OrderTracker oldTracker, OrderTracker newTracker)
		{
			XDebug.Log($"Replacing order tracker ({oldTracker.GetType().Name}) with ({newTracker.GetType().Name}) for order: {oldTracker.TrackingData.orderId}");
			RemoveOrderFromTracking(oldTracker.TrackingData.orderId);
			StartTracker(newTracker);
		}

		private static void StartTracker(OrderTracker tracker)
		{
			XDebug.Log($"Order tracker '{tracker.GetType().Name}' started for order: {tracker.TrackingData.orderId}");
			Trackers.Add(tracker.TrackingData.orderId, tracker);
			tracker.Start();
		}
	}
}