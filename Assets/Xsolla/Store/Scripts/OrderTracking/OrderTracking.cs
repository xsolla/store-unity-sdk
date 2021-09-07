using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
    public class OrderTracking : MonoSingleton<OrderTracking>
	{
		private readonly List<OrderTrackingData> _trackingOrders = new List<OrderTrackingData>();
		private readonly Dictionary<int, Coroutine> _orderTrackingCoroutines = new Dictionary<int, Coroutine>();
		
		private bool IsTrackByPaystationCallback
		{
			get
			{
#if UNITY_WEBGL
				if ((Application.platform == RuntimePlatform.WebGLPlayer) && (XsollaSettings.InAppBrowserEnabled))
					return true;
#endif
				//else
				return false;
			}
		}

		private bool IsTrackByBrowserUrlChange
		{
			get
			{
				return BrowserHelper.Instance.InAppBrowser != null;
			}
		}

		private bool IsTrackByPolling
		{
			get => Application.platform == RuntimePlatform.Android || BrowserHelper.Instance.InAppBrowser == null;
		}

		private void Start()
		{
#if UNITY_WEBGL
			XsollaWebCallbacks.PaymentStatusUpdate += () => 
			{
				if (IsTrackByPaystationCallback)
				{
					foreach (var order in _trackingOrders)
					{
						CheckOrderDone(order.OrderId, BrowserHelper.ClosePaystationWidget);
					}
				}

			};

			XsollaWebCallbacks.PaymentCancel += () =>
			{
				while (_trackingOrders.Count > 0)
				{
					RemoveOrderFromTracking(_trackingOrders[0].OrderId);
				}
			};
#endif
		}

		public void AddOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			var order = _trackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order != null)
				return;

			order = new OrderTrackingData
			{
				ProjectId = projectId,
				OrderId = orderId,
				SuccessCallback = onSuccess,
				ErrorCallback = onError
			};

			_trackingOrders.Add(order);

			if (IsTrackByPolling)
			{
				var coroutine = StartCoroutine(CheckOrderForeverUntilDone(orderId));
				_orderTrackingCoroutines.Add(orderId, coroutine);
			}

			if (IsTrackByBrowserUrlChange)
			{
#if UNITY_EDITOR || UNITY_STANDALONE
				BrowserHelper.Instance.InAppBrowser.AddUrlChangeHandler(url =>
				{
					var regex = new Regex(@"(?<=secure.xsolla.com/paystation)(.+?)(?=status)");
					var matches = regex.Matches(url);
					if (matches.Count > 0)
					{
						CheckOrderDone(orderId);
					}

					// handle case when manual/automatic redirect was triggered
					if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.status, out var status))
					{
						if (status != "done")
						{
							// occurs when redirect triggered automatically with any status without delay
							var coroutine = StartCoroutine(CheckOrderWithRepeatCount(orderId));
							_orderTrackingCoroutines.Add(orderId, coroutine);
						}

						BrowserHelper.Instance.Close(0.1f);
					}
				});
#endif
			}
		}

		public void AddOrderForTrackingUntilDone(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			var order = _trackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order != null)
				return;

			order = new OrderTrackingData
			{
				ProjectId = projectId,
				OrderId = orderId,
				SuccessCallback = onSuccess,
				ErrorCallback = onError
			};

			_trackingOrders.Add(order);

			var coroutine = StartCoroutine(CheckOrderForeverUntilDone(orderId));
			_orderTrackingCoroutines.Add(orderId, coroutine);
		}

		public void RemoveOrderFromTracking(int orderId)
		{
			var order = _trackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order != null)
			{
				_trackingOrders.Remove(order);

				if (IsTrackByPolling)
				{
					if (_orderTrackingCoroutines.ContainsKey(orderId))
					{
						StopCoroutine(_orderTrackingCoroutines[orderId]);
					}
				}
			}
		}

		private void CheckOrderDone(int orderId, Action onDone = null)
		{
			if (Token.Instance == null)
			{
				Debug.LogWarning("Invalid token in order status polling. Polling stopped.");
				RemoveOrderFromTracking(orderId);
				return;
			}

			var order = _trackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order == null)
				return;

			XsollaStore.Instance.CheckOrderStatus(
				order.ProjectId,
				order.OrderId,
				status =>
				{
					// Prevent double check
					if (_trackingOrders.Any(x => x.OrderId == order.OrderId))
					{
						if (status.status == "paid" || status.status == "done")
						{
							order.SuccessCallback?.Invoke();
							RemoveOrderFromTracking(order.OrderId);
							onDone?.Invoke();
						}
					}
				},
				order.ErrorCallback
			);
		}

		private IEnumerator CheckOrderForeverUntilDone(int orderId)
		{
			var isDone = false;
			while (!isDone)
			{
				yield return new WaitForSeconds(3f);
				CheckOrderDone(orderId, () => isDone = true);
			}
		}

		private IEnumerator CheckOrderWithRepeatCount(int orderId, int repeatCount = 10)
		{
			var isDone = false;
			while (repeatCount > 0 && !isDone)
			{
				yield return new WaitForSeconds(3f);
				CheckOrderDone(orderId, () =>
				{
					isDone = true;
					repeatCount--;
				});
			}
		}
	}
}
