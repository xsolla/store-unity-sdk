using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xsolla.Core;
using UnityEngine;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private readonly Dictionary<int, Coroutine> OrderTrackingCoroutines = new Dictionary<int, Coroutine>();

		public readonly List<OrderTrackingData> TrackingOrders = new List<OrderTrackingData>();

		private bool IsTrackingByPaystationCallbacks
		{
			get
			{
#if UNITY_WEBGL
				if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					if (XsollaSettings.InAppBrowserEnabled)
					{
						return true;
					}
				}
#endif
				return false;
			}
		}

		private bool IsTrackingByBrowserUrlChange
		{
			get
			{
#if UNITY_EDITOR || UNITY_STANDALONE
				if (XsollaSettings.InAppBrowserEnabled)
				{
					return true;
				}
#endif
				return false;
			}
		}

		private bool IsTrackingForeverUntilDone
		{
			get
			{
				if (!XsollaSettings.InAppBrowserEnabled || Application.platform == RuntimePlatform.Android)
				{
					return true;
				}
				
				return false;
			}
		}

		private void Start()
		{
#if UNITY_WEBGL
			XsollaWebCallbacks.PaymentStatusUpdate += () => 
			{
				if (IsTrackingByPaystationCallbacks)
				{
					foreach (var order in TrackingOrders)
					{
						CheckOrderDone(order.OrderId, BrowserHelper.ClosePaystationWidget);
					}
				}

			};

			XsollaWebCallbacks.PaymentCancel += () =>
			{
				while (TrackingOrders.Count > 0)
				{
					RemoveOrderFromTracking(TrackingOrders[0].OrderId);
				}
			};
#endif
		}

		public void AddOrderForTracking(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			var order = TrackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order != null)
				return;

			order = new OrderTrackingData
			{
				ProjectId = projectId,
				OrderId = orderId,
				SuccessCallback = onSuccess,
				ErrorCallback = onError
			};

			TrackingOrders.Add(order);

			if (IsTrackingForeverUntilDone)
			{
				var coroutine = StartCoroutine(CheckOrderForeverUntilDone(orderId));
				OrderTrackingCoroutines.Add(orderId, coroutine);
			}

			if (IsTrackingByBrowserUrlChange)
			{
#if UNITY_EDITOR || UNITY_STANDALONE
				var browser = BrowserHelper.Instance.GetLastBrowser();
				browser.GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent += (xsollaBrowser, url) =>
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
							OrderTrackingCoroutines.Add(orderId, coroutine);
						}

						Destroy(BrowserHelper.Instance, 0.1F);
					}
				};
#endif
			}
		}

		public void RemoveOrderFromTracking(int orderId)
		{
			var order = TrackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order != null)
			{
				TrackingOrders.Remove(order);

				if (IsTrackingForeverUntilDone)
				{
					if (OrderTrackingCoroutines.ContainsKey(orderId))
					{
						StopCoroutine(OrderTrackingCoroutines[orderId]);
					}
				}
			}
		}

		private void CheckOrderDone(int orderId, Action onDone = null)
		{
			var order = TrackingOrders.FirstOrDefault(x => x.OrderId == orderId);
			if (order == null)
				return;

			CheckOrderStatus(
				order.ProjectId,
				order.OrderId,
				status =>
				{
					// Prevent double check
					if (TrackingOrders.Any(x => x.OrderId == order.OrderId))
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