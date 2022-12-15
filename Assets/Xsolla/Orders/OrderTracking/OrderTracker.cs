using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public abstract class OrderTracker
	{
		private readonly OrderTrackingData _trackingData;
		private bool _isCheckInProgress = false;

		public OrderTrackingData TrackingData => _trackingData;

		public abstract void Start();
		public abstract void Stop();

		protected void RemoveSelfFromTracking()
		{
			OrderTracking.Instance.RemoveOrderFromTracking(TrackingData.orderId);
		}

		protected void CheckOrderStatus(Action onDone = null, Action onCancel = null, Action<Error> onError = null)
		{
			if (Token.Instance == null)
			{
				Debug.LogWarning("No Token in order status check. Check cancelled");
				onCancel?.Invoke();
				return;
			}

			if (_isCheckInProgress) // Prevent double check
				return;

			_isCheckInProgress = true;

			var orderId = TrackingData.orderId;
			XsollaOrders.Instance.CheckOrderStatus(
				TrackingData.projectId,
				TrackingData.orderId,
				status =>
				{
					_isCheckInProgress = false;
					HandleOrderStatus(status.status, onDone, onCancel);
				},
				error =>
				{
					_isCheckInProgress = false;
					onError?.Invoke(error);
				}
			);
		}

		protected void HandleOrderStatus(string status, Action onDone, Action onCancel)
		{
			switch (status)
			{
				case "done":
					onDone?.Invoke();
					break;
				case "canceled":
					onCancel?.Invoke();
					break;
			}
		}

		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			return OrderTracking.Instance.StartCoroutine(routine);
		}

		protected void StopCoroutine(Coroutine routine)
		{
			OrderTracking.Instance.StopCoroutine(routine);
		}

		protected OrderTracker(OrderTrackingData trackingData)
		{
			_trackingData = trackingData;
		}
	}
}
