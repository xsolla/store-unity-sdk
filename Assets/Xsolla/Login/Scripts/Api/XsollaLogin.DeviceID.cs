using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_JWT_DEVICE_ID_AUTH =
			"https://login.xsolla.com/api/login/device/{0}?projectId={1}{2}&with_logout={3}";

		private const string URL_OAUTH_DEVICE_ID_AUTH =
			"https://login.xsolla.com/api/oauth2/login/device/{0}?client_id={1}&response_type=code&redirect_uri=https://login.xsolla.com/api/blank&state={2}&scope=offline";

		private const string URL_ADD_USERNAME_EMAIL =
			"https://login.xsolla.com/api/users/me/link_email_password?login_url={0}";

		private const string URL_GET_USERS_DEVICES =
			"https://login.xsolla.com/api/users/me/devices";

		private const string URL_DEVICES_LINKING =
			"https://login.xsolla.com/api/users/me/devices/{0}";

		/// <summary>
		/// Authenticates a user via a particular device ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Device ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-auth-via-device-id"/>.
		/// <see cref="https://developers.xsolla.com/login-api/auth/oauth-20/oauth-20-auth-via-device-id/"/>.
		/// <param name="deviceType">Type of the device.</param>
		/// <param name="deviceName">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Device ID: For Android it is an ANDROID_ID constant. For iOS it is an identifierForVendor property.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the 'user JWT' > `payload` claim. Used only for JWT authorization type.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 characters. Used only for OAuth2.0 authorization type.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AuthViaDeviceID(DeviceType deviceType, string deviceName, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LoginDeviceIdRequest(deviceName, deviceId);

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
					Token = response.token;
					onSuccess?.Invoke(Token);
				},
				onError, Error.LoginErrors);
		}

		private void OAuthAuthViaDeviceID(string deviceType, LoginDeviceIdRequest requestBody, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var clientId = XsollaSettings.OAuthClientId;
			var stateUrlParam = state ?? DEFAULT_OAUTH_STATE;

			var url = string.Format(URL_OAUTH_DEVICE_ID_AUTH, deviceType, clientId, stateUrlParam);

			WebRequestHelper.Instance.PostRequest<LoginJwtJsonResponse, LoginDeviceIdRequest>(SdkType.Login, url, requestBody,
				onComplete: (response) =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string code))
						XsollaLogin.Instance.ExchangeCodeToToken(code, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
					else
						onError?.Invoke(Error.UnknownError);
				},
				onError, Error.LoginErrors);
		}

		/// <summary>
		/// Adds the username/email and password authentication to the existing user account. This call is used if the account is created via device ID or phone number.
		/// </summary>
		/// <remarks>Swagger method name:<c>Add username/email auth to account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-profile/add-username-email-auth-to-account/"/>.
		/// <param name="email">User email.</param>
		/// <param name="password">User password.</param>
		/// <param name="username">Username.</param>
		/// <param name="promoEmailAgreement">User consent to receive the newsletter.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AddUsernameEmailAuthToAccount(string email, string password, string username, int? promoEmailAgreement = null, Action<bool> onSuccess = null, Action<Error> onError = null)
		{
			var requestBody = new AddUsernameAndEmailRequest(email, password, promoEmailAgreement, username);
			var loginUrl = XsollaSettings.CallbackUrl;
			var url = string.Format(URL_ADD_USERNAME_EMAIL, loginUrl);

			Action<AddUsernameAndEmailResponse> onComplete = response =>
			{
				onSuccess?.Invoke(response.email_confirmation_required);
			};

			WebRequestHelper.Instance.PostRequest<AddUsernameAndEmailResponse, AddUsernameAndEmailRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token),
				onComplete: onComplete,
				onError: onError);
		}

		/// <summary>
		/// Gets a list of user's devices.
		/// </summary>
		/// <remarks>Swagger method name:<c>Get user's devices</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/get-users-devices/"/>.
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserDevices(Action<List<UserDeviceInfo>> onSuccess = null, Action<Error> onError = null)
		{
			var url = URL_GET_USERS_DEVICES;

			Action<List<UserDeviceInfo>> onComplete = responseItems =>
			{
				onSuccess?.Invoke(responseItems);
			};

			WebRequestHelper.Instance.GetRequest<List<UserDeviceInfo>>(SdkType.Login, url, WebRequestHeader.AuthHeader(Token),
				onComplete: onComplete,
				onError: onError);
		}

		/// <summary>
		/// Links the specified device to the user account. To enable authentication via device ID and linking, contact your Account Manager.
		/// </summary>
		/// <remarks>Swagger method name:<c>Link device to account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/link-device-to-account/"/>.
		/// <param name="deviceType">Type of the device.</param>
		/// <param name="deviceName">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Device ID: For Android it is an ANDROID_ID constant. For iOS it is an identifierForVendor property.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void LinkDeviceToAccount(DeviceType deviceType, string deviceName, string deviceId, Action onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LoginDeviceIdRequest(deviceName, deviceId);
			var url = string.Format(URL_DEVICES_LINKING, deviceTypeAsString);

			WebRequestHelper.Instance.PostRequest<LoginDeviceIdRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token),
				onSuccess,
				onError);
		}

		/// <summary>
		/// Unlinks the specified device from the user account. To enable authentication via device ID and unlinking, contact your Account Manager.
		/// </summary>
		/// <remarks>Swagger method name:<c>Unlink the device from account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/unlink-device-from-account/"/>.
		/// <param name="id">Device ID of the device you want to unlink. It is generated by the Xsolla Login server. It is not the same as the `device_id` parameter from Auth via device ID call.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UnlinkDeviceFromAccount(int id, Action onSuccess = null, Action<Error> onError = null)
		{
			var url = string.Format(URL_DEVICES_LINKING, id);

			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token),
				onSuccess,
				onError);
		}
	}
}
