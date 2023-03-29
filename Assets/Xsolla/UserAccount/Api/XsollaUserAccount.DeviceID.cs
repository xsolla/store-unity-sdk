using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_ADD_USERNAME_EMAIL =
			"https://login.xsolla.com/api/users/me/link_email_password?login_url={0}";

		private const string URL_GET_USERS_DEVICES =
			"https://login.xsolla.com/api/users/me/devices";

		private const string URL_DEVICES_LINKING =
			"https://login.xsolla.com/api/users/me/devices/{0}";

		/// <summary>
		/// Adds a username, email address, and password, that can be used for authentication, to the current account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email.</param>
		/// <param name="promoEmailAgreement">Whether the user gave consent to receive the newsletters.</param>
		/// <param name="onSuccess">Called after successful email and password linking.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void AddUsernameEmailAuthToAccount(string username, string password, string email, int? promoEmailAgreement = null, Action<bool> onSuccess = null, Action<Error> onError = null)
		{
			var requestBody = new AddUsernameAndEmailRequest(username, password, email, promoEmailAgreement);
			var loginUrl = RedirectUtils.GetRedirectUrl();
			var url = string.Format(URL_ADD_USERNAME_EMAIL, loginUrl);

			Action<AddUsernameAndEmailResponse> onComplete = response =>
			{
				onSuccess?.Invoke(response.email_confirmation_required);
			};

			WebRequestHelper.Instance.PostRequest<AddUsernameAndEmailResponse, AddUsernameAndEmailRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token.Instance),
				onComplete: onComplete,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => AddUsernameEmailAuthToAccount(username, password, email, promoEmailAgreement, onSuccess, onError)),
				errorsToCheck: ErrorCheckType.RegistrationErrors);
		}

		/// <summary>
		/// Gets a list of user's devices.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="onSuccess">Called after users devices data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetUserDevices(Action<List<UserDeviceInfo>> onSuccess = null, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest<List<UserDeviceInfo>>(SdkType.Login, URL_GET_USERS_DEVICES, WebRequestHeader.AuthHeader(Token.Instance),
				onComplete: onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetUserDevices(onSuccess, onError)));
		}

		/// <summary>
		/// Links the specified device to the current user account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="deviceType">Type of the device. Can be `android` or `ios`.</param>
		/// <param name="device">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Platform specific unique device ID.
		/// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.<br/>
		/// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.<br/>
		/// </param>
		/// <param name="onSuccess">Called after successful linking of the device.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void LinkDeviceToAccount(DeviceType deviceType, string device, string deviceId, Action onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LinkDeviceRequest(device, deviceId);
			var url = string.Format(URL_DEVICES_LINKING, deviceTypeAsString);

			WebRequestHelper.Instance.PostRequest<LinkDeviceRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token.Instance),
				onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => LinkDeviceToAccount(deviceType, device, deviceId, onSuccess, onError)));
		}

		/// <summary>
		/// Unlinks the specified device from the current user account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="id">Platform specific unique device ID.<br/>
		/// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.<br/>
		/// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.</param>
		/// <param name="onSuccess">Called after successful unlinking of the device.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void UnlinkDeviceFromAccount(int id, Action onSuccess = null, Action<Error> onError = null)
		{
			var url = string.Format(URL_DEVICES_LINKING, id);

			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance),
				onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UnlinkDeviceFromAccount(id, onSuccess, onError)));
		}
	}
}
