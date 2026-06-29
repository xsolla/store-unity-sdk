using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Bridge
{
	public class SocialAuthTest : MonoBehaviour
	{
		[SerializeField] private string loginId = "";
		[SerializeField] private int oauthClientId = 0;
		[SerializeField] private string socialNetworkName = "GOOGLE";

		public void Execute()
		{
			if (string.IsNullOrEmpty(loginId) || oauthClientId == 0)
			{
				loginId = XsollaSettings.LoginId;
				oauthClientId = XsollaSettings.OAuthClientId;
			}

			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var context = activity.Call<AndroidJavaObject>("getApplicationContext");

			var socialConfig = new AndroidJavaObject(
				"com.xsolla.android.bridge.login.XLogin$SocialConfig",
				"", "", "", "", "");

			var builder = new AndroidJavaObject("com.xsolla.android.bridge.login.LoginConfig$OauthBuilder");
			builder.Call<AndroidJavaObject>("setSocialConfig", socialConfig);
			builder.Call<AndroidJavaObject>("setProjectId", loginId);
			builder.Call<AndroidJavaObject>("setOauthClientId", oauthClientId);
			var loginConfig = builder.Call<AndroidJavaObject>("build");

			var xlogin = new AndroidJavaClass("com.xsolla.android.bridge.login.XLogin");
			xlogin.CallStatic("init", context, loginConfig);
			Debug.Log("AndroidSocialAuthTest: XLogin initialized");

			var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.bridge.login.social.SocialNetwork");
			var socialNetwork = socialNetworkClass.GetStatic<AndroidJavaObject>(socialNetworkName);

			var proxyActivity = new AndroidJavaClass("com.xsolla.android.bridge.login.social.SocialAuthBridgeActivity");
			proxyActivity.CallStatic("perform", activity, socialNetwork, new SocialAuthCallback(xlogin));

			Debug.Log("AndroidSocialAuthTest: browser should open for " + socialNetworkName + " auth...");
		}

		private class SocialAuthCallback : AndroidJavaProxy
		{
			private readonly AndroidJavaClass _xlogin;

			public SocialAuthCallback(AndroidJavaClass xlogin)
				: base("com.xsolla.android.bridge.login.callback.AuthCallback")
			{
				_xlogin = xlogin;
			}

			public void onSuccess()
			{
				var token = _xlogin.CallStatic<string>("getToken");
				var refreshToken = _xlogin.CallStatic<string>("getRefreshToken");
				Debug.Log("AndroidSocialAuthTest: AUTH SUCCESS!");
				Debug.Log("AndroidSocialAuthTest: accessToken=" + token);
				Debug.Log("AndroidSocialAuthTest: refreshToken=" + refreshToken);
			}

			public void onError(AndroidJavaObject throwable, string errorMessage)
			{
				Debug.LogError("AndroidSocialAuthTest: AUTH ERROR: " + errorMessage);
			}
		}
	}
}
