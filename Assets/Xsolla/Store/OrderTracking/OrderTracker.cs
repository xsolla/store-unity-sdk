using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	public abstract class OrderTracker
	{
		public readonly OrderTrackingData trackingData;

		protected readonly OrderTracking orderTracking;

		private readonly List<int> currentRequests;

		public abstract void Start();

		public abstract void Stop();

		protected void RemoveSelfFromTracking()
		{
			orderTracking.RemoveOrderFromTracking(trackingData.OrderId);
		}

		protected void CheckOrderStatus(Action onDone = null, Action onCancel = null, Action<Error> onError = null)
		{
			if (Token.Instance == null)
			{
				onError?.Invoke(new Error(ErrorType.InvalidToken, "Invalid token in order status polling. Polling stopped"));
				return;
			}

			var orderId = trackingData.OrderId;
			if (currentRequests.Contains(orderId)) // Prevent double check
				return;

			currentRequests.Add(orderId);

			XsollaStore.Instance.CheckOrderStatus(
				trackingData.ProjectId,
				trackingData.OrderId,
				status =>
				{
					currentRequests.Remove(orderId);
					HandleOrderStatus(status.status, onDone, onCancel);
				},
				error =>
				{
					currentRequests.Remove(orderId);
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
			return orderTracking.StartCoroutine(routine);
		}

		protected void StopCoroutine(Coroutine routine)
		{
			orderTracking.StopCoroutine(routine);
		}

		protected OrderTracker(OrderTrackingData trackingData, OrderTracking orderTracking)
		{
			this.trackingData = trackingData;
			this.orderTracking = orderTracking;
			currentRequests = new List<int>();
		}
	}
}