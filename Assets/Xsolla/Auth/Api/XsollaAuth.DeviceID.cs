using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_DEVICE_ID_AUTH =
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
		/// <param name="payload">[OBSOLETE] Were used only for JWT authorization type.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 characters. Used only for OAuth2.0 authorization type.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AuthViaDeviceID(DeviceType deviceType, string device, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LoginDeviceIdRequest(device, deviceId);
			var clientId = XsollaSettings.OAuthClientId;
			var stateUrlParam = state ?? DEFAULT_OAUTH_STATE;
			var redirectParam = RedirectUtils.GetRedirectUrl();

			var url = string.Format(URL_DEVICE_ID_AUTH, deviceType, clientId, stateUrlParam, redirectParam);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, LoginDeviceIdRequest>(SdkType.Login, url, requestBody,
				onComplete: (response) =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string code))
						XsollaAuth.Instance.ExchangeCodeToToken(code, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
					else
						onError?.Invoke(Error.UnknownError);
				},
				onError, ErrorCheckType.LoginErrors);
		}
	}
}
