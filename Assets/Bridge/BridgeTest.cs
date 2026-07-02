using UnityEngine;

namespace Xsolla.Bridge
{
	public class BridgeTest : MonoBehaviour
	{
		public void Execute()
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
			TestLoginConfig();
			TestStartSocialAuth();

			var authCallback = new TestAuthCallback();
			bridgeFacadeClass.CallStatic("testAuthCallback", authCallback);

			var browserCallback = new TestBrowserCallback();
			bridgeFacadeClass.CallStatic("testBrowserCallback", browserCallback);

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

		private static void TestLoginConfig()
		{
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var context = activity.Call<AndroidJavaObject>("getApplicationContext");

			var socialConfig = new AndroidJavaObject(
				"com.xsolla.android.bridge.login.XLogin$SocialConfig",
				"test-fb-app-id",
				"test-fb-client-token",
				"test-google-server-id",
				"test-wechat-app-id",
				"test-qq-app-id");

			var builder = new AndroidJavaObject("com.xsolla.android.bridge.login.LoginConfig$OauthBuilder");
			builder.Call<AndroidJavaObject>("setSocialConfig", socialConfig);
			builder.Call<AndroidJavaObject>("setProjectId", "test-project-id");
			builder.Call<AndroidJavaObject>("setOauthClientId", 123);
			var loginConfig = builder.Call<AndroidJavaObject>("build");

			var xlogin = new AndroidJavaClass("com.xsolla.android.bridge.login.XLogin");
			xlogin.CallStatic("init", context, loginConfig);
			Debug.Log("ANDROID_BRIDGE_TEST: XLogin.init() OK");

			var token = xlogin.CallStatic<string>("getToken");
			var refreshToken = xlogin.CallStatic<string>("getRefreshToken");
			Debug.Log("ANDROID_BRIDGE_TEST: XLogin.getToken()=" + token + " getRefreshToken()=" + refreshToken);
		}

		private static void TestStartSocialAuth()
		{
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.bridge.login.social.SocialNetwork");
			var google = socialNetworkClass.GetStatic<AndroidJavaObject>("GOOGLE");

			var xlogin = new AndroidJavaClass("com.xsolla.android.bridge.login.XLogin");
			xlogin.CallStatic("startSocialAuth", activity, google, new TestStartSocialCallback());
			Debug.Log("ANDROID_BRIDGE_TEST: startSocialAuth called — waiting for callback...");
		}

		private class TestStartSocialCallback : AndroidJavaProxy
		{
			public TestStartSocialCallback() : base("com.xsolla.android.bridge.login.callback.StartSocialCallback") { }

			public void onAuthStarted()
			{
				Debug.Log("ANDROID_BRIDGE_TEST: StartSocialCallback.onAuthStarted() — browser opened!");
			}

			public void onError(AndroidJavaObject throwable, string errorMessage)
			{
				Debug.Log("ANDROID_BRIDGE_TEST: StartSocialCallback.onError(): " + errorMessage);
			}
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

		private class TestBrowserCallback : AndroidJavaProxy
		{
			public TestBrowserCallback() : base("com.xsolla.android.bridge.payments.callback.BrowserCallback") { }

			public void onBrowserClosed(bool isManually)
			{
				Debug.Log($"ANDROID_BRIDGE_TEST: BrowserCallback.onBrowserClosed(isManually={isManually}) received!");
			}
		}
	}
}