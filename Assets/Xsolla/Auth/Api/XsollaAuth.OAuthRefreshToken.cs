using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_OAUTH_GENERATE_JWT = "https://login.xsolla.com/api/oauth2/token";
		private bool _subscribed = false;

		/// <summary>
		/// Refreshes the existing token using previously saved OAuth2.0 refresh token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Generate JWT</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/generate-jwt"/>
		/// <param name="onSuccessExchange">Success operation callback. Contains new token.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RefreshOAuthToken(Action<string> onSuccess, Action<Error> onError)
		{
			var refreshToken = TokenRefresh.Instance.RefreshToken;
			RefreshOAuthToken(refreshToken, onSuccess, onError);
		}

		/// <summary>
		/// Refreshes the existing token using provided OAuth2.0 refresh token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Generate JWT</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/generate-jwt"/>
		/// <param name="refreshToken">Refresh token.</param>
		/// <param name="onSuccessExchange">Success operation callback. Contains new token.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RefreshOAuthToken(string refreshToken, Action<string> onSuccess, Action<Error> onError)
		{
			if (EnvironmentDefiner.IsAndroid && Token.Instance.FromSocialNetwork())
			{
				TryRefreshAndroidSocial(onSuccess, onError);
				return;
			}

			if (string.IsNullOrEmpty(refreshToken))
			{
				var message = "Invalid refresh token";
				Debug.LogWarning(message);
				onError?.Invoke(new Error(ErrorType.InvalidToken, errorMessage: message));
				return;
			}

			var clientId = XsollaSettings.OAuthClientId;
			var redirectUri = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var requestData = new WWWForm();

			requestData.AddField("client_id", clientId);
			requestData.AddField("grant_type", "refresh_token");
			requestData.AddField("refresh_token", refreshToken);
			requestData.AddField("redirect_uri", redirectUri);

			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse>(
				sdkType: SdkType.Login,
				url: URL_OAUTH_GENERATE_JWT,
				data: requestData,
				onComplete: response => ProcessOAuthResponse(response, onSuccess),
				onError: error => onError?.Invoke(error));
		}

		private void TryRefreshAndroidSocial(Action<string> onSuccess, Action<Error> onError)
		{
			using (var helper = new AndroidSDKSocialAuthHelper())
			{
				if (!helper.IsRefreshSocialTokenPossible)
				{
					Debug.Log("Android social token refresh is not available at the moment");
					onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed));
					return;
				}

				Action<string> onSuccessRefresh = newToken =>
				{
					var surrogateResponse = new LoginOAuthJsonResponse()
					{
						access_token = newToken,
						refresh_token = string.Empty,
					};

					ProcessOAuthResponse(surrogateResponse, onSuccess);
				};

				Action<Error> onRefreshError = error =>
				{
					if (error != null)
						Debug.LogError(error.errorMessage);

					onError?.Invoke(error);
				};

				helper.TryRefreshSocialToken(onSuccessRefresh, onError);
			}
		}

		/// <summary>
		/// Exchanges the code received in the authentication call for the JWT token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Generate JWT</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/generate-jwt"/>
		/// <param name="code">Access code received from several other OAuth2.0 requests (example: code from social network auth).</param>
		/// <param name="onSuccessExchange">Success operation callback. Contains exchanged token.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
		{
			var clientId = XsollaSettings.OAuthClientId;
			var redirectUri = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;

			var requestData = new WWWForm();

			requestData.AddField("grant_type", "authorization_code");
			requestData.AddField("client_id", clientId);
			requestData.AddField("redirect_uri", redirectUri);
			requestData.AddField("code", code);

			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse>(
				sdkType: SdkType.Login,
				url: URL_OAUTH_GENERATE_JWT,
				data: requestData,
				onComplete: response => ProcessOAuthResponse(response, onSuccessExchange),
				onError: error => onError?.Invoke(error));
		}

		private void ProcessOAuthResponse(LoginOAuthJsonResponse response, Action<string> onSuccessToken)
		{
			Token.Instance = Token.Create(response.access_token);
			SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, response.access_token);
			TokenRefresh.Instance.RefreshToken = response.refresh_token;

			onSuccessToken?.Invoke(response.access_token);
		}

		private void SetupOAuthRefresh()
		{
			if (_subscribed) return;

			_subscribed = true;
			TokenRefresh.Instance.OnInvalidToken += HandleInvalidToken;
		}

		private void TeardownOAuthRefresh()
		{
			if (!_subscribed) return;

			_subscribed = false;
			TokenRefresh.Instance.OnInvalidToken -= HandleInvalidToken;
		}

		private void HandleInvalidToken(Action repeatCall, Action<Error> onError)
		{
			RefreshOAuthToken(
				onSuccess: _ => repeatCall?.Invoke(),
				onError: onError);
		}
	}
}
