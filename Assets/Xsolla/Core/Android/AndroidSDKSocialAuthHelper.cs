using System;
using UnityEngine;
using Xsolla.Core.Android;

namespace Xsolla.Core
{
	public class AndroidSDKSocialAuthHelper : IDisposable
	{
		private AndroidHelper _androidHelper;
		private bool _invalidationFlag;
		private AndroidJavaClass _xlogin;

		public AndroidSDKSocialAuthHelper()
		{
			GetXsollaSettings(
				out string loginID,
				out AuthorizationType authorizationType,
				out bool invalidationFlag,
				out int OAuthClientId,
				out string facebookAppId,
				out string googleServerId,
				out string wechatAppId,
				out string qqAppId);

			_androidHelper = new AndroidHelper();
			_invalidationFlag = invalidationFlag;

			try
			{
				var xlogin = new AndroidJavaClass("com.xsolla.android.login.XLogin");
				var context = _androidHelper.ApplicationContext;

				var socialConfig = new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig", facebookAppId, googleServerId, wechatAppId, qqAppId);

				AndroidJavaObject loginConfig;
				AndroidJavaObject loginConfigBuilder;

				loginConfigBuilder = authorizationType == AuthorizationType.JWT
					? new AndroidJavaObject("com.xsolla.android.login.LoginConfig$JwtBuilder")
					: new AndroidJavaObject("com.xsolla.android.login.LoginConfig$OauthBuilder");

				loginConfigBuilder.Call<AndroidJavaObject>("setProjectId", loginID);
				loginConfigBuilder.Call<AndroidJavaObject>("setSocialConfig", socialConfig);

				if (authorizationType == AuthorizationType.OAuth2_0)
					loginConfigBuilder.Call<AndroidJavaObject>("setOauthClientId", OAuthClientId);

				loginConfig = loginConfigBuilder.Call<AndroidJavaObject>("build");

				xlogin.CallStatic("init", context, loginConfig);

				_xlogin = xlogin;
			}
			catch (Exception ex)
			{
				throw new AggregateException($"AndroidSDKSocialAuthHelper.Ctor: {ex.Message}", ex);
			}
		}

		public void PerformSocialAuth(SocialProvider socialProvider)
		{
			var providerName = socialProvider.ToString().ToUpper();

			Debug.Log($"Trying android social auth for '{providerName}'");

			try
			{
				var unitySDKHelper = new AndroidJavaClass("com.xsolla.android.login.XLogin$Unity");
				var actvity = _androidHelper.CurrentActivity;
				var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.login.social.SocialNetwork");
				var socialNetworkObject = socialNetworkClass.GetStatic<AndroidJavaObject>(providerName);
				var invalidationFlag = _invalidationFlag;

				unitySDKHelper.CallStatic("authSocial", actvity, socialNetworkObject, invalidationFlag);
			}
			catch (Exception ex)
			{
				throw new AggregateException($"AndroidSDKSocialAuthHelper.PerformSocialAuth: {ex.Message}", ex);
			}
		}

		public bool IsRefreshSocialTokenPossible
		{
			get
			{
				var canRefresh = default(bool);

				try
				{
					canRefresh = _xlogin.CallStatic<bool>("canRefreshToken");
				}
				catch (Exception ex)
				{
					Debug.LogError($"AndroidSDKSocialAuthHelper.IsRefreshSocialTokenPossible: {ex.Message}");
					canRefresh = false;
				}

				return canRefresh;
			}
		}

		public bool IsSocialTokenExpired
		{
			get
			{
				var isTokenExpired = default(bool);

				try
				{
					//Argument type of long is required, but is used only for JWT expiration check, which is not the case
					isTokenExpired = _xlogin.CallStatic<bool>("isTokenExpired", (object) 0L);
				}
				catch (Exception ex)
				{
					Debug.LogError($"AndroidSDKSocialAuthHelper.IsSocialTokenExpired: {ex.Message}");
					isTokenExpired = false;
				}

				return isTokenExpired;
			}
		}

		public bool TryRefreshSocialToken(Action<string> onSuccessRefresh, Action<Error> onError)
		{
			try
			{
				if (IsRefreshSocialTokenPossible)
				{
					var callback = new AndroidSDKRefreshTokenCallback();

					callback.OnSuccess = onSuccessRefresh;
					callback.OnError = onError;

					_xlogin.CallStatic("refreshToken", callback);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				Debug.LogError($"AndroidSDKSocialAuthHelper.TryRefreshSocialToken: {ex.Message}");
				return false;
			}
		}

		private void GetXsollaSettings(out string loginID, out AuthorizationType authorizationType, out bool invalidationFlag, out int OAuthClientId, out string facebookAppId, out string googleServerId, out string wechatAppId, out string qqAppId)
		{
			loginID = XsollaSettings.LoginId;
			authorizationType = XsollaSettings.AuthorizationType;

			if (authorizationType == AuthorizationType.JWT)
				invalidationFlag = XsollaSettings.InvalidateExistingSessions;
			else /*if (authorizationType == AuthorizationType.OAuth2_0)*/
				invalidationFlag = true;

			OAuthClientId = XsollaSettings.OAuthClientId;
			facebookAppId = XsollaSettings.FacebookAppId;
			googleServerId = XsollaSettings.GoogleServerId;
			wechatAppId = XsollaSettings.WeChatAppId;
			qqAppId = XsollaSettings.QQAppId;
		}

		public void Dispose()
		{
			_androidHelper.Dispose();
		}
	}
}
