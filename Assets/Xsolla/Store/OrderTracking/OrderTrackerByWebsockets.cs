using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;
using Xsolla.Core;

namespace Xsolla.Store
{
	public class OrderTrackerByWebsockets : OrderTracker
	{
		private WebSocket webSocket;

		private Coroutine timeoutCoroutine;

		private OnMainThreadExecutor onMainThreadExecutor;

		private const string URL_PATTERN = "wss://store-ws.xsolla.com/sub/order/status?order_id={0}&project_id={1}";

		private const int TIME_OUT_DURATION = 300;

		public override void Start()
		{
			onMainThreadExecutor = orderTracking.GetComponent<OnMainThreadExecutor>();
			if (!onMainThreadExecutor)
				onMainThreadExecutor = orderTracking.gameObject.AddComponent<OnMainThreadExecutor>();

			var url = string.Format(URL_PATTERN, trackingData.OrderId, trackingData.ProjectId);
			webSocket = new WebSocket(url);
			webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

			webSocket.OnMessage += OnWebSocketMessage;
			webSocket.OnClose += OnWebSocketClose;
			webSocket.OnError += OnWebSocketError;
			webSocket.Connect();

			timeoutCoroutine = StartCoroutine(WaitTimeout());
		}

		public override void Stop()
		{
			if (webSocket != null)
			{
				webSocket.OnMessage -= OnWebSocketMessage;
				webSocket.OnClose -= OnWebSocketClose;
				webSocket.OnError -= OnWebSocketError;
				webSocket = null;
			}

			if (timeoutCoroutine != null)
				StopCoroutine(timeoutCoroutine);
		}

		private IEnumerator WaitTimeout()
		{
			yield return new WaitForSeconds(TIME_OUT_DURATION);
			ReplaceSelfByShortPollingTracker();
		}

		private void ReplaceSelfByShortPollingTracker()
		{
			var shortPollingTracker = new OrderTrackerByShortPolling(trackingData, orderTracking);
			orderTracking.ReplaceTracker(this, shortPollingTracker);
		}

		private void OnWebSocketMessage(object sender, MessageEventArgs args)
		{
			onMainThreadExecutor.Enqueue(() =>
			{
				var status = JsonConvert.DeserializeObject<OrderStatus>(args.Data);

				HandleOrderStatus(
					status.status,
					onDone: () =>
					{
						trackingData.SuccessCallback?.Invoke();
						RemoveSelfFromTracking();
					},
					onCancel: RemoveSelfFromTracking
				);
			});
		}

		private void OnWebSocketClose(object sender, CloseEventArgs args)
		{
			onMainThreadExecutor.Enqueue(ReplaceSelfByShortPollingTracker);
		}

		private void OnWebSocketError(object sender, ErrorEventArgs args)
		{
			onMainThreadExecutor.Enqueue(ReplaceSelfByShortPollingTracker);
		}

		public OrderTrackerByWebsockets(OrderTrackingData trackingData, OrderTracking orderTracking) : base(trackingData, orderTracking)
		{
		}
	}
}