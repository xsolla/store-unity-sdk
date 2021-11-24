using System.Collections;
using UnityEngine;

namespace Xsolla.Store
{
	public class OrderTrackerByShortPolling : OrderTracker
	{
		private const float CHECK_STATUS_COOLDOWN = 3f;

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
			while (true)
			{
				yield return new WaitForSeconds(CHECK_STATUS_COOLDOWN);

				CheckOrderStatus(
					onDone: () =>
					{
						if (trackingData != null && trackingData.SuccessCallback != null)
							trackingData.SuccessCallback.Invoke();
						RemoveSelfFromTracking();
					},
					onCancel: RemoveSelfFromTracking,
					onError: error =>
					{
						if (trackingData != null && trackingData.ErrorCallback != null)
							trackingData.ErrorCallback.Invoke(error);
						RemoveSelfFromTracking();
					}
				);
			}
		}

		public OrderTrackerByShortPolling(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}
