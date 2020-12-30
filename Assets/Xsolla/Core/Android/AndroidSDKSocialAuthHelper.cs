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
				out string callbackURL,
				out AuthorizationType authorizationType,
				out bool invalidationFlag,
				out int OAuthClientId,
				out string facebookAppId,
				out string googleServerId);

			_androidHelper = new AndroidHelper();
			_invalidationFlag = invalidationFlag;

			try
			{
				var xlogin = new AndroidJavaClass("com.xsolla.android.login.XLogin");
				var context = _androidHelper.ApplicationContext;
				var socialConfigBuilder = new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig$Builder");
            
				socialConfigBuilder.Call<AndroidJavaObject>("facebookAppId", facebookAppId);
				socialConfigBuilder.Call<AndroidJavaObject>("googleServerId", googleServerId);

				var socialConfig = socialConfigBuilder.Call<AndroidJavaObject>("build");

				if (authorizationType == AuthorizationType.JWT)
				{
					if (!string.IsNullOrEmpty(callbackURL))
						xlogin.CallStatic("initJwt", context, loginID, callbackURL, socialConfig);
					else
						xlogin.CallStatic("initJwt", context, loginID, socialConfig);
				}
				else/*if (authorizationType == AuthorizationType.OAuth2_0)*/
				{
					if (!string.IsNullOrEmpty(callbackURL))
						xlogin.CallStatic("initOauth", context, loginID, OAuthClientId, callbackURL, socialConfig);
					else
						xlogin.CallStatic("initOauth", context, loginID, OAuthClientId, socialConfig);
				}

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
					isTokenExpired = isTokenExpired = _xlogin.CallStatic<bool>("isTokenExpired", (object)0L);
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

		private void GetXsollaSettings(out string loginID, out string callbackURL, out AuthorizationType authorizationType, out bool invalidationFlag, out int OAuthClientId, out string facebookAppId, out string googleServerId)
		{
			loginID = XsollaSettings.LoginId;
			callbackURL = XsollaSettings.CallbackUrl;
			authorizationType = XsollaSettings.AuthorizationType;

			if (authorizationType == AuthorizationType.JWT)
				invalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled;
			else/*if (authorizationType == AuthorizationType.OAuth2_0)*/
				invalidationFlag = true;

			OAuthClientId = XsollaSettings.OAuthClientId;
			facebookAppId = XsollaSettings.FacebookAppId;
			googleServerId = XsollaSettings.GoogleServerId;
		}

		public void Dispose()
		{
			_androidHelper.Dispose();
		}
	}
}
