using UnityEngine;

namespace Xsolla.Bridge
{
	public class PaymentsTest : MonoBehaviour
	{
		public void Execute(string token)
		{
			const bool isSandbox = true;
			var redirectScheme = "app";
			var redirectHost = "xpayment." + Application.identifier;
			var paystationVersion = 4;
			var activityType = "WebView";

			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			var proxyActivity = new AndroidJavaClass("com.xsolla.android.bridge.payments.ui.PayStationBridgeActivity");
			proxyActivity.CallStatic("perform",
				activity,
				token,
				isSandbox,
				redirectScheme,
				redirectHost,
				paystationVersion,
				new PaymentsBrowserCallback(),
				activityType);

			Debug.Log("PaymentsTest: perform() called — PayStation should open...");
		}

		private class PaymentsBrowserCallback : AndroidJavaProxy
		{
			public PaymentsBrowserCallback() : base("com.xsolla.android.bridge.payments.callback.BrowserCallback") { }

			public void onBrowserClosed(bool isManually)
			{
				Debug.Log("PaymentsTest: onBrowserClosed(isManually=" + isManually + ")");
			}
		}
	}
}