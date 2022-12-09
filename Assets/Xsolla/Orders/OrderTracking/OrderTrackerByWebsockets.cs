using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class OrderTrackerByWebsockets : OrderTracker
	{
		private const string URL_PATTERN = "wss://store-ws.xsolla.com/sub/order/status?order_id={0}&project_id={1}";
		private const int TIME_OUT_DURATION = 300;

		private WebSocket _webSocket;
		private Coroutine _timeoutCoroutine;
		private OnMainThreadExecutor _onMainThreadExecutor;

		public override void Start()
		{
			_onMainThreadExecutor = OrderTracking.Instance.GetComponent<OnMainThreadExecutor>();
			if (!_onMainThreadExecutor)
				_onMainThreadExecutor = OrderTracking.Instance.gameObject.AddComponent<OnMainThreadExecutor>();

			var url = string.Format(URL_PATTERN, TrackingData.orderId, TrackingData.projectId);
			_webSocket = new WebSocket(url);
			_webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

			_webSocket.OnMessage += OnWebSocketMessage;
			_webSocket.OnClose += OnWebSocketClose;
			_webSocket.OnError += OnWebSocketError;
			_webSocket.Connect();

			_timeoutCoroutine = base.StartCoroutine(WaitTimeout());
		}

		public override void Stop()
		{
			if (_webSocket != null)
			{
				_webSocket.OnMessage -= OnWebSocketMessage;
				_webSocket.OnClose -= OnWebSocketClose;
				_webSocket.OnError -= OnWebSocketError;
				_webSocket = null;
			}

			if (_timeoutCoroutine != null)
				base.StopCoroutine(_timeoutCoroutine);
		}

		private IEnumerator WaitTimeout()
		{
			yield return new WaitForSeconds(TIME_OUT_DURATION);
			ReplaceSelfByShortPollingTracker();
		}

		private void ReplaceSelfByShortPollingTracker()
		{
			var shortPollingTracker = new OrderTrackerByShortPolling(TrackingData);
			OrderTracking.Instance.ReplaceTracker(this, shortPollingTracker);
		}

		private void OnWebSocketMessage(object sender, MessageEventArgs args)
		{
			_onMainThreadExecutor.Enqueue(() =>
			{
				var status = JsonConvert.DeserializeObject<OrderStatus>(args.Data);

				HandleOrderStatus(
					status.status,
					onDone: () =>
					{
						TrackingData.successCallback?.Invoke();
						RemoveSelfFromTracking();
					},
					onCancel: RemoveSelfFromTracking
				);
			});
		}

		private void OnWebSocketClose(object sender, CloseEventArgs args)
		{
			_onMainThreadExecutor.Enqueue(ReplaceSelfByShortPollingTracker);
		}

		private void OnWebSocketError(object sender, ErrorEventArgs args)
		{
			_onMainThreadExecutor.Enqueue(ReplaceSelfByShortPollingTracker);
		}

		public OrderTrackerByWebsockets(OrderTrackingData trackingData) : base(trackingData)
		{
		}
	}
}
