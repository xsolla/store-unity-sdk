using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Bridge
{
	public class WidgetAuthTest : MonoBehaviour
	{
		[SerializeField] private string loginId = "";
		[SerializeField] private int oauthClientId = 0;
		[SerializeField] private string locale = "";

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
			Debug.Log("WidgetAuthTest: XLogin initialized");

			var widgetActivity = new AndroidJavaClass("com.xsolla.android.bridge.login.social.XsollaWidgetBridgeActivity");
			widgetActivity.CallStatic("perform", activity, new WidgetAuthCallback(xlogin), locale);

			Debug.Log("WidgetAuthTest: browser should open for Xsolla Widget auth...");
		}

		private class WidgetAuthCallback : AndroidJavaProxy
		{
			private readonly AndroidJavaClass _xlogin;

			public WidgetAuthCallback(AndroidJavaClass xlogin)
				: base("com.xsolla.android.bridge.login.callback.AuthCallback")
			{
				_xlogin = xlogin;
			}

			public void onSuccess()
			{
				var token = _xlogin.CallStatic<string>("getToken");
				var refreshToken = _xlogin.CallStatic<string>("getRefreshToken");
				Debug.Log("WidgetAuthTest: AUTH SUCCESS!");
				Debug.Log("WidgetAuthTest: accessToken=" + token);
				Debug.Log("WidgetAuthTest: refreshToken=" + refreshToken);
			}

			public void onError(AndroidJavaObject throwable, string errorMessage)
			{
				Debug.LogError("WidgetAuthTest: AUTH ERROR: " + errorMessage);
			}
		}
	}
}
