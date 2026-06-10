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

			var authCallback = new TestAuthCallback();
			bridgeFacadeClass.CallStatic("testAuthCallback", authCallback);

			Debug.Log("END ANDROID_BRIDGE_TEST");
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