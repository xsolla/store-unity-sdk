using UnityEngine;

namespace Xsolla.TechDemo
{
	public class AndroidBridgeTest : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log("START ANDROID_BRIDGE_TEST");
			var bridgeFacadeClass = new AndroidJavaClass("com.xsolla.android.bridge.XsollaBridge");
			bridgeFacadeClass.CallStatic("init");

			var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.bridge.login.social.SocialNetwork");
			var socialNetwork = socialNetworkClass.GetStatic<AndroidJavaObject>("DISCORD");
			bridgeFacadeClass.CallStatic("testSocialNetwork", socialNetwork);

			bridgeFacadeClass.CallStatic("testSetToken", "test-access-token-123");
			var token = bridgeFacadeClass.CallStatic<string>("testGetToken");
			Debug.Log("ANDROID_BRIDGE_TEST: testGetToken=" + token);

			TestAnalyticsUtils();

			var authCallback = new TestAuthCallback();
			bridgeFacadeClass.CallStatic("testAuthCallback", authCallback);

			Debug.Log("END ANDROID_BRIDGE_TEST");
		}

		private static void TestAnalyticsUtils()
		{
			var loginAnalytics = new AndroidJavaClass("com.xsolla.android.bridge.login.util.AnalyticsUtils");
			loginAnalytics.SetStatic("gameEngine", "unity");
			loginAnalytics.SetStatic("gameEngineVersion", Application.unityVersion);
			var engine = loginAnalytics.GetStatic<string>("gameEngine");
			var version = loginAnalytics.GetStatic<string>("gameEngineVersion");
			Debug.Log("ANDROID_BRIDGE_TEST: login.AnalyticsUtils gameEngine=" + engine + " gameEngineVersion=" + version);

			var paymentsAnalytics = new AndroidJavaClass("com.xsolla.android.bridge.payments.util.AnalyticsUtils");
			paymentsAnalytics.SetStatic("gameEngine", "unity");
			paymentsAnalytics.SetStatic("gameEngineVersion", Application.unityVersion);
			var engine2 = paymentsAnalytics.GetStatic<string>("gameEngine");
			Debug.Log("ANDROID_BRIDGE_TEST: payments.AnalyticsUtils gameEngine=" + engine2);
		}

		private class TestAuthCallback : AndroidJavaProxy
		{
			public TestAuthCallback() : base("com.xsolla.android.bridge.login.callback.AuthCallback") { }

			public void onSuccess()
			{
				Debug.Log("ANDROID_BRIDGE_TEST: AuthCallback.onSuccess() received!");
			}

			public void onError(AndroidJavaObject throwable, string errorMessage)
			{
				Debug.Log($"ANDROID_BRIDGE_TEST: AuthCallback.onError() received: {errorMessage}");
			}
		}
	}
}