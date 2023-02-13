using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_DEVICE_ID_AUTH =
			"https://login.xsolla.com/api/oauth2/login/device/{0}?client_id={1}&response_type=code&state={2}&redirect_uri={3}&scope=offline";

		/// <summary>
		/// Authenticates the user via a particular device ID.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="deviceType">Type of the device. Can be `android` and `ios`.</param>
		/// <param name="device">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Platform specific unique device ID.
		/// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.
		/// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.
		/// </param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="onSuccess">Called after successful user authentication via the device ID.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void AuthViaDeviceID(DeviceType deviceType, string device, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeToLower = deviceType.ToString().ToLower();
			var requestBody = new LoginDeviceIdRequest(device, deviceId);
			var clientId = XsollaSettings.OAuthClientId;
			var stateUrlParam = state ?? DEFAULT_OAUTH_STATE;
			var redirectParam = RedirectUtils.GetRedirectUrl();

			var url = string.Format(URL_DEVICE_ID_AUTH, deviceTypeToLower, clientId, stateUrlParam, redirectParam);

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
