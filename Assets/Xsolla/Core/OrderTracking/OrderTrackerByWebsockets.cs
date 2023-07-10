using System;
using UnityEngine;
using WebSocketSharp;
using Object = UnityEngine.Object;

namespace Xsolla.Core
{
	internal class OrderTrackerByWebsockets : OrderTracker
	{
		private const string BASE_URL = "wss://store-ws.xsolla.com/sub/order/status";

		private WebSocket webSocket;
		private float startTime;

		private static MainThreadExecutor MainThreadExecutor;

		public OrderTrackerByWebsockets(OrderTrackingData trackingData) : base(trackingData) { }

		public override void Start()
		{
			if (!MainThreadExecutor)
			{
				MainThreadExecutor = new GameObject("Websockets MainThreadExecutor").AddComponent<MainThreadExecutor>();
				Object.DontDestroyOnLoad(MainThreadExecutor.gameObject);
			}

			var url = new UrlBuilder(BASE_URL)
				.AddParam("order_id", TrackingData.orderId)
				.AddParam("project_id", XsollaSettings.StoreProjectId)
				.Build();

			webSocket = new WebSocket(url);
			webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

			webSocket.OnMessage += OnWebSocketMessage;
			webSocket.OnClose += OnWebSocketInterrupt;
			webSocket.OnError += OnWebSocketInterrupt;
			webSocket.Connect();

			startTime = Time.realtimeSinceStartup;
		}

		public override void Stop()
		{
			if (webSocket == null)
				return;

			webSocket.OnMessage -= OnWebSocketMessage;
			webSocket.OnClose -= OnWebSocketInterrupt;
			webSocket.OnError -= OnWebSocketInterrupt;
			webSocket.Close();
			webSocket = null;
		}

		private void OnWebSocketMessage(object sender, MessageEventArgs args)
		{
			MainThreadExecutor.Enqueue(() =>
			{
				OrderStatus status = null;

				try
				{
					status = ParseUtils.FromJson<OrderStatus>(args.Data);
				}
				catch (Exception ex)
				{
					XDebug.LogError($"Could not parse WebSocket message with exception: {ex.Message}");
				}

				if (status == null)
				{
					XDebug.LogError("WebSocket message order status is null");
					return;
				}

				XDebug.Log($"WebSocket status message. OrderId:'{status.order_id}' Status:'{status.status}'");

				HandleOrderStatus(
					status.status,
					() =>
					{
						TrackingData.successCallback?.Invoke();
						RemoveSelfFromTracking();
					},
					RemoveSelfFromTracking
				);
			});
		}

		private void OnWebSocketInterrupt(object sender, EventArgs args)
		{
			MainThreadExecutor.Enqueue(() =>
			{
				switch (args)
				{
					case CloseEventArgs closeArgs:
						XDebug.LogWarning($"WebSocket was closed with the code:'{closeArgs.Code}', reason:'{closeArgs.Reason}', wasClean:'{closeArgs.WasClean}'");
						break;
					case ErrorEventArgs errorArgs:
						XDebug.LogError($"WebSocket was closed with the exception:'{errorArgs.Exception.Message}', message:'{errorArgs.Message}'");
						break;
					default:
						XDebug.LogWarning($"WebSocket was closed with the unexpected argument '{args}'");
						break;
				}

				var webSocketTTL = Time.realtimeSinceStartup - startTime;
				if (webSocketTTL >= Constants.WEB_SOCKETS_TIMEOUT)
				{
					XDebug.Log($"WebSocket TTL '{webSocketTTL}' is OK, replacing an interrupted websocket with a new one");

					var newTracker = new OrderTrackerByWebsockets(TrackingData);
					if (OrderTrackingService.ReplaceTracker(this, newTracker))
						XDebug.Log($"Tracker for an order {TrackingData.orderId} is replaced with a new one");
					else
						XDebug.LogError($"Failed to replace a tracker for an order {TrackingData.orderId}");
				}
				else
				{
					XDebug.LogWarning($"WebSocket TTL '{webSocketTTL}' is lower than expected, switching to short polling");

					var shortPollingTracker = new OrderTrackerByShortPolling(TrackingData);
					if (OrderTrackingService.ReplaceTracker(this, shortPollingTracker))
						XDebug.Log($"Tracker for an order {TrackingData.orderId} is replaced with short polling");
					else
						XDebug.LogError($"Failed to replace the tracker for an order {TrackingData.orderId}");
				}
			});
		}
	}
}