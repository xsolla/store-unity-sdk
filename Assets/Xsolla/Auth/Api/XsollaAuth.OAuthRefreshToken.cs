using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_OAUTH_GENERATE_JWT = "https://login.xsolla.com/api/oauth2/token";
		
		private Coroutine _refreshTokenCoroutine = null;

#if UNITY_ANDROID
		private const int ANDROID_TOKEN_REFRESH_RATE = 3600;
#endif

		/// <summary>
		/// Returns 'true' during refresh token process, or false otherwise.
		/// </summary>
		public bool IsOAuthTokenRefreshInProgress { get; private set; } = false;

		private void InitOAuth2_0()
		{
			Token.TokenChanged += () =>
			{
				if (IsOAuthTokenRefreshInProgress)
					return;

				if (Token.Instance == null)
					StopTokenRefreshAndClearData();
				else
					SetNextTokenRefresh();
			};

			SetNextTokenRefresh();
		}

		private void StopTokenRefreshAndClearData()
		{
			if (_refreshTokenCoroutine != null)
				StopCoroutine(_refreshTokenCoroutine);

			ClearTokenRefreshData();
			IsOAuthTokenRefreshInProgress = false;

			Debug.Log("Token refresh cancelled");
		}

		private void ClearTokenRefreshData()
		{
			DeleteToken(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);
			DeleteToken(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME);
		}

		private void SetNextTokenRefresh()
		{
			var refreshTime = 0;
			var secondsLeftUntilExpiration = LoadTokenExpirationTime();

			if (secondsLeftUntilExpiration > 0)
				refreshTime = secondsLeftUntilExpiration;
			else
			{
				Debug.Log("OAuth token expired and needs to be refreshed");
				IsOAuthTokenRefreshInProgress = true;
			}

			SetRefreshAfter(refreshTime);
		}

		private int LoadTokenExpirationTime()
		{
			var loadedExpirationTime = PlayerPrefs.GetString(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME, defaultValue: string.Empty);

			if (string.IsNullOrEmpty(loadedExpirationTime))
			{
				Debug.Log("Could not load OAuth token expiration time");
				return 0;
			}

			DateTime parsedExpirationTime;

			if (DateTime.TryParse(loadedExpirationTime, out parsedExpirationTime) == false)
			{
				Debug.LogError("Could not parse loaded OAuth token expiration time");
				return 0;
			}

			return (int)(parsedExpirationTime - DateTime.Now).TotalSeconds;
		}

		private void SetRefreshAfter(int seconds)
		{
			if (_refreshTokenCoroutine != null)
				StopCoroutine(_refreshTokenCoroutine);

			_refreshTokenCoroutine = StartCoroutine(RefreshTokenAfter(seconds));
		}

		private IEnumerator RefreshTokenAfter(int seconds)
		{
			Debug.Log($"Next OAuth token refresh attempt is in: {seconds} seconds");
			yield return new WaitForSeconds(seconds);
			RefreshToken();
		}

		private void RefreshToken()
		{
			if (Token.Instance == null && Token.Load() == false)
			{
				Debug.Log("Token refresh is not available due to lack of active token");
				ClearTokenRefreshData();
				IsOAuthTokenRefreshInProgress = false;
				return;
			}

			IsOAuthTokenRefreshInProgress = true;
			Debug.Log("OAuth token refresh is in progress");

#if UNITY_ANDROID
			if (Token.Instance.FromSocialNetwork())
			{
				TryRefreshAndroidSocial();
				return;
			}
#endif

			var clientId = XsollaSettings.OAuthClientId;
			var refreshToken = PlayerPrefs.GetString(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN, string.Empty);

			if (!string.IsNullOrEmpty(refreshToken))
			{
				var requestData = new WWWForm();

				requestData.AddField("client_id", clientId);
				requestData.AddField("grant_type", "refresh_token");
				requestData.AddField("refresh_token", refreshToken);
				requestData.AddField("redirect_uri", DEFAULT_REDIRECT_URI);

				SendOAuthGenerateJwtRequest(requestData);
			}
			else
			{
				Debug.LogWarning("Could not load saved refresh token");
				IsOAuthTokenRefreshInProgress = false;
			}
		}

#if UNITY_ANDROID
		private void TryRefreshAndroidSocial()
		{
			using (var helper = new AndroidSDKSocialAuthHelper())
			{
				if (!helper.IsRefreshSocialTokenPossible)
				{
					Debug.Log("Android social token refresh is not available at the moment");
					IsOAuthTokenRefreshInProgress = false;
					return;
				}

				Action<string> onSuccessRefresh = newToken =>
				{
					var surrogateResponse = new LoginOAuthJsonResponse()
					{
						access_token = newToken,
						expires_in = ANDROID_TOKEN_REFRESH_RATE,
						refresh_token = string.Empty,
					};

					ProcessOAuthResponse(surrogateResponse);
				};

				Action<Error> onError = error =>
				{
					if (error != null)
						Debug.LogError(error.errorMessage);

					IsOAuthTokenRefreshInProgress = false;
				};

				IsOAuthTokenRefreshInProgress = helper.TryRefreshSocialToken(onSuccessRefresh, onError);
			}
		}
#endif

		private void SendOAuthGenerateJwtRequest(WWWForm requestData, Action<string> onSuccessGenerate = null, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse>(
				sdkType: SdkType.Login,
				url: URL_OAUTH_GENERATE_JWT,
				data: requestData,
				onComplete: response => ProcessOAuthResponse(response, onSuccessGenerate),
				onError: error => { IsOAuthTokenRefreshInProgress = false; Debug.Log($"Generate JWT failed: {error.errorMessage}"); onError?.Invoke(error); });
		}

		private void ProcessOAuthResponse(LoginOAuthJsonResponse response, Action<string> onSuccessToken = null)
		{
			Debug.Log("OAuth response received");

			Token.Instance = Token.Create(response.access_token);
			SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, response.access_token);
			PlayerPrefs.SetString(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN, response.refresh_token);

			//Set expiration time for expiration check on next application start
			var expirationTime = DateTime.Now.AddSeconds(response.expires_in);
			PlayerPrefs.SetString(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME, expirationTime.ToString());

			//Set next token refresh while application active
			SetRefreshAfter(response.expires_in);

			IsOAuthTokenRefreshInProgress = false;
			onSuccessToken?.Invoke(response.access_token);
		}

		/// <summary>
		/// Use this call:
		///To get a user JWT.
		///To refresh the JWT when it expires.Works only if scope=offline is passed in the registration or authentication call.
		///To get a server JWT.The user participation isn't needed.
		///Usage of this call depends on the value of the grant_type parameter.
		/// </summary>
		/// <remarks> Swagger method name:<c>Generate JWT</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/generate-jwt"/>
		/// <param name="code">Access code received from several other OAuth2.0 requests (example: code from social network auth)</param>
		/// <param name="onSuccessExchange">Success operation callback. Contains exchanged token.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
		{
			IsOAuthTokenRefreshInProgress = true;

			var clientId = XsollaSettings.OAuthClientId;
			var redirectUri = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;

			var requestData = new WWWForm();

			requestData.AddField("grant_type", "authorization_code");
			requestData.AddField("client_id", clientId);
			requestData.AddField("redirect_uri", redirectUri);
			requestData.AddField("code", code);

			SendOAuthGenerateJwtRequest(requestData, onSuccessExchange, onError);
		}
	}
}
