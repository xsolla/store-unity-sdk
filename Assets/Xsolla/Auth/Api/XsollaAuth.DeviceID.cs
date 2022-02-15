using System;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_JWT_DEVICE_ID_AUTH =
			"https://login.xsolla.com/api/login/device/{0}?projectId={1}{2}&with_logout={3}";

		private const string URL_OAUTH_DEVICE_ID_AUTH =
			"https://login.xsolla.com/api/oauth2/login/device/{0}?client_id={1}&response_type=code&state={2}&redirect_uri={3}&scope=offline";

		/// <summary>
		/// Authenticates a user via a particular device ID. To enable authentication, contact your Account Manager.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Device ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-auth-via-device-id"/>.
		/// <see cref="https://developers.xsolla.com/login-api/auth/oauth-20/oauth-20-auth-via-device-id/"/>.
		/// <param name="deviceType">Type of the device.</param>
		/// <param name="device">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Device ID: For Android it is an ANDROID_ID constant. For iOS it is an identifierForVendor property.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT authorization type.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 characters. Used only for OAuth2.0 authorization type.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AuthViaDeviceID(DeviceType deviceType, string device, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LoginDeviceIdRequest(device, deviceId);

			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				JwtAuthViaDeviceID(deviceTypeAsString, requestBody, payload, onSuccess, onError);
			}
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				OAuthAuthViaDeviceID(deviceTypeAsString, requestBody, state, onSuccess, onError);
			}
		}

		private void JwtAuthViaDeviceID(string deviceType, LoginDeviceIdRequest requestBody, string payload = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var projectId = XsollaSettings.LoginId;
			var payloadUrlParam = (payload != null) ? $"&payload={payload}" : string.Empty;
			var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";

			var url = string.Format(URL_JWT_DEVICE_ID_AUTH, deviceType, projectId, payloadUrlParam, with_logout);

			WebRequestHelper.Instance.PostRequest<TokenEntity, LoginDeviceIdRequest>(SdkType.Login, url, requestBody,
				onComplete: (response) =>
				{
					Token.Instance = Token.Create(response.token);
					onSuccess?.Invoke(Token.Instance);
				},
				onError, Error.LoginErrors);
		}

		private void OAuthAuthViaDeviceID(string deviceType, LoginDeviceIdRequest requestBody, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var clientId = XsollaSettings.OAuthClientId;
			var stateUrlParam = state ?? DEFAULT_OAUTH_STATE;
			var redirectParam = (!string.IsNullOrEmpty(XsollaSettings.CallbackUrl)) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;

			var url = string.Format(URL_OAUTH_DEVICE_ID_AUTH, deviceType, clientId, stateUrlParam, redirectParam);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, LoginDeviceIdRequest>(SdkType.Login, url, requestBody,
				onComplete: (response) =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string code))
						XsollaAuth.Instance.ExchangeCodeToToken(code, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
					else
						onError?.Invoke(Error.UnknownError);
				},
				onError, Error.LoginErrors);
		}
	}
}
