using System;
using Newtonsoft.Json;
using WebSocketSharp;
using Xsolla.Core;
using UnityEngine;

namespace Xsolla.Orders
{
	public class OrderTrackerByWebsockets : OrderTracker
	{
		private const string URL_PATTERN = "wss://store-ws.xsolla.com/sub/order/status?order_id={0}&project_id={1}";
		private const float EXPECTED_TTL = 300f;

		private WebSocket _webSocket;
		private OnMainThreadExecutor _onMainThreadExecutor;
		private float _startTime;

		public override void Start()
		{
			_onMainThreadExecutor = OrderTracking.Instance.GetComponent<OnMainThreadExecutor>();
			if (!_onMainThreadExecutor)
				_onMainThreadExecutor = OrderTracking.Instance.gameObject.AddComponent<OnMainThreadExecutor>();

			var url = string.Format(URL_PATTERN, base.TrackingData.orderId, base.TrackingData.projectId);
			_webSocket = new WebSocket(url);
			_webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

			_webSocket.OnMessage += OnWebSocketMessage;
			_webSocket.OnClose += OnWebSocketInterrupt;
			_webSocket.OnError += OnWebSocketInterrupt;
			_webSocket.Connect();

			_startTime = Time.time;
		}

		public override void Stop()
		{
			if (_webSocket != null)
			{
				_webSocket.OnMessage -= OnWebSocketMessage;
				_webSocket.OnClose -= OnWebSocketInterrupt;
				_webSocket.OnError -= OnWebSocketInterrupt;
				_webSocket.Close();
				_webSocket = null;
			}
		}

		private void OnWebSocketMessage(object sender, MessageEventArgs args)
		{
			_onMainThreadExecutor.Enqueue(() =>
			{
				OrderStatus status = null;

				try {
					status = JsonConvert.DeserializeObject<OrderStatus>(args.Data);
				} catch (System.Exception ex) {
					Debug.LogError($"Could not parse WebSocket message with exception: {ex.Message}");
				}

				if (status == null) {
					Debug.LogError($"WebSocket message order status is null");
					return;
				}

				Debug.Log($"WebSocket status message. OrderID:'{status.order_id}' Status:'{status.status}'");

				base.HandleOrderStatus(
					status.status,
					onDone: () =>
					{
						base.TrackingData.successCallback?.Invoke();
						RemoveSelfFromTracking();
					},
					onCancel: RemoveSelfFromTracking
				);
			});
		}

		private void OnWebSocketInterrupt(object sender, EventArgs args)
		{
			_onMainThreadExecutor.Enqueue(() =>
			{
				if (args is CloseEventArgs closeArgs)
					Debug.LogWarning($"WebSocket was closed with the code:'{closeArgs.Code}', reason:'{closeArgs.Reason}', wasClean:'{closeArgs.WasClean}'");
				else if (args is ErrorEventArgs errorArgs)
					Debug.LogError($"WebSocket was closed with the exception:'{errorArgs.Exception.Message}', message:'{errorArgs.Message}'");
				else
					Debug.LogWarning($"WebSocket was closed with the unexpected argument '{args}'");

				var webSocketTTL = Time.time - _startTime;

				if (webSocketTTL >= EXPECTED_TTL)
				{
					Debug.Log($"WebSocket TTL '{webSocketTTL}' is OK, replacing an interrupted websocket with a new one");

					var newTracker = new OrderTrackerByWebsockets(base.TrackingData);

					if (OrderTracking.Instance.ReplaceTracker(this, newTracker))
						Debug.Log($"Traker for an order {base.TrackingData.orderId} is replaced with a new one");
					else
						Debug.LogError($"Failed to relace a tracker for an order {base.TrackingData.orderId}");
				}
				else
				{
					Debug.LogWarning($"WebSocket TTL '{webSocketTTL}' is lower than expected, switching to short polling");

					var shortPollingTracker = new OrderTrackerByShortPolling(base.TrackingData);

					if (OrderTracking.Instance.ReplaceTracker(this, shortPollingTracker))
						Debug.Log($"Traker for an order {base.TrackingData.orderId} is replaced with short polling");
					else
						Debug.LogError($"Failed to relace the tracker for an order {base.TrackingData.orderId}");
				}
			});
		}

		public OrderTrackerByWebsockets(OrderTrackingData trackingData) : base(trackingData)
		{
		}
	}
}
