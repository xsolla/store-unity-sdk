using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackerByShortPolling : OrderTracker
	{
		public static float ShortPollingInterval {get;set;} = 3f;
		public static float ShortPollingSecondsLimit {get;set;} = 600f;

		private Coroutine checkStatusCoroutine;

		public override void Start()
		{
			checkStatusCoroutine = StartCoroutine(CheckOrderStatus());
		}

		public override void Stop()
		{
			if (checkStatusCoroutine != null)
				StopCoroutine(checkStatusCoroutine);
		}

		private IEnumerator CheckOrderStatus()
		{
			var interval = new WaitForSeconds(ShortPollingInterval);
			var limit = Time.time + ShortPollingSecondsLimit;

			Action onDone = () =>
			{
				trackingData.successCallback?.Invoke();
				base.RemoveSelfFromTracking();
			};

			Action onCancel = () =>
			{
				base.RemoveSelfFromTracking();
			};

			Action<Error> onError = error =>
			{
				trackingData.errorCallback?.Invoke(error);
				base.RemoveSelfFromTracking();
			};

			while (true)
			{
				yield return interval;
				CheckOrderStatus(onDone, onCancel, onError);

				if (Time.time > limit) {
					onError?.Invoke(new Error(ErrorType.TimeLimitReached, errorMessage:"Polling time limit reached"));
					break;
				}
			}
		}

		public OrderTrackerByShortPolling(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}
