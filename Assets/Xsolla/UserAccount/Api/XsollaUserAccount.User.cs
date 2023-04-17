using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me";
		private const string URL_USER_PHONE = "https://login.xsolla.com/api/users/me/phone";
		private const string URL_USER_PICTURE = "https://login.xsolla.com/api/users/me/picture";
		private const string URL_SEARCH_USER = "https://login.xsolla.com/api/users/search/by_nickname?nickname={0}&offset={1}&limit={2}";
		private const string URL_USER_PUBLIC_INFO = "https://login.xsolla.com/api/users/{0}/public";
		private const string URL_USER_CHECK_AGE = "https://login.xsolla.com/api/users/age/check";
		private const string URL_USER_GET_EMAIL = "https://login.xsolla.com/api/users/me/email";

		/// <summary>
		/// Updates the specified user’s information. Changes are made on the user data storage side.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="info">User information.</param>
		/// <param name="onSuccess">Called after successful user details modification.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.PatchRequest(SdkType.Login, URL_USER_INFO, info, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UpdateUserInfo(Token.Instance, info, onSuccess, onError)));
		}

		public void UpdateUserInfo(UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			UpdateUserInfo(Token.Instance, info, onSuccess, onError);
		}

		/// <summary>
		/// Searches users by the nickname parameter and gets a list of them. Search can be performed instantly when the user starts entering the search parameter.
		/// NOTE: User can search only 1 time per second.
		/// </summary>
		/// <param name="token">User authorization token.</param>
		/// <param name="nickname">The search string that may contain: Nickname only, Tag only, Nickname and tag together</param>
		/// <param name="offset">Number of the elements from which the list is generated.</param>
		/// <param name="limit">Maximum number of users that are returned at a time.</param>
		/// <param name="onSuccess">Called after user search is successfully completed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void SearchUsers(string token, string nickname, uint offset, uint limit, Action<FoundUsers> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_SEARCH_USER, nickname, offset, limit);
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => SearchUsers(Token.Instance, nickname, offset, limit, onSuccess, onError)));
		}

		public void SearchUsers(string nickname, uint offset, uint limit, Action<FoundUsers> onSuccess, Action<Error> onError = null)
		{
			SearchUsers(Token.Instance, nickname, offset, limit, onSuccess, onError);
		}

		/// <summary>
		/// Returns specified user public profile information.
		/// </summary>
		/// <param name="token">User authorization token.</param>
		/// <param name="userId">User identifier of public profile information to be received.</param>
		/// <param name="onSuccess">Called after user profile data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetPublicInfo(string token, string userId, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_USER_PUBLIC_INFO, userId);
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetPublicInfo(Token.Instance, userId, onSuccess, onError)));
		}

		public void GetPublicInfo(string userId, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			GetPublicInfo(Token.Instance, userId, onSuccess, onError);
		}

		/// <summary>
		/// Returns user phone number that is used for two-factor authentication.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="onSuccess">Called after user phone number was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="UpdateUserPhoneNumber"/>
		/// <seealso href="DeleteUserPhoneNumber"/>
		public void GetUserPhoneNumber(string token, Action<string> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_PHONE, WebRequestHeader.AuthHeader(token),
				onComplete: (UserPhoneNumber number) => onSuccess?.Invoke(number.phone_number),
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetUserPhoneNumber(Token.Instance, onSuccess, onError)));
		}

		public void GetUserPhoneNumber(Action<string> onSuccess, Action<Error> onError)
		{
			GetUserPhoneNumber(Token.Instance, onSuccess, onError);
		}

		/// <summary>
		/// Changes the user’s phone number that is used for two-factor authentication. Changes are made on the user data storage side (server-side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="phoneNumber">Updated user phone number according to national conventions.</param>
		/// <param name="onSuccess">Called after user phone number was successfully modified.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="GetUserPhoneNumber"/>
		/// <seealso href="DeleteUserPhoneNumber"/>
		public void UpdateUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var request = new UserPhoneNumber { phone_number = phoneNumber };
			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_PHONE, request, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UpdateUserPhoneNumber(Token.Instance, phoneNumber, onSuccess, onError)));
		}

		public void UpdateUserPhoneNumber(string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			UpdateUserPhoneNumber(Token.Instance, phoneNumber, onSuccess, onError);
		}

		/// <summary>
		/// Deletes the user’s phone number that is used for two-factor authentication. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="phoneNumber">User phone number for removal.</param>
		/// <param name="onSuccess">Called after the user phone number was successfully removed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="GetUserPhoneNumber"/>
		/// <seealso href="UpdateUserPhoneNumber"/>
		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var url = $"{URL_USER_PHONE}/{phoneNumber}";
			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => DeleteUserPhoneNumber(Token.Instance, phoneNumber, onSuccess, onError)));
		}

		public void DeleteUserPhoneNumber(string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			DeleteUserPhoneNumber(Token.Instance, phoneNumber, onSuccess, onError);
		}

		/// <summary>
		/// Changes the user’s avatar. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="pictureData">User profile picture in the binary format.</param>
		/// <param name="onSuccess">Called after the user profile picture was successfully modified.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="DeleteUserPicture"/>
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			var headers = new List<WebRequestHeader>(){
				WebRequestHeader.AuthHeader(token),
				new WebRequestHeader(){
					Name = "Content-type",
					Value = $"multipart/form-data; boundary ={boundary}"
				}
			};
			WebRequestHelper.Instance.PostUploadRequest(SdkType.Login, URL_USER_PICTURE, pictureData, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UploadUserPicture(Token.Instance, pictureData, boundary, onSuccess, onError)));
		}

		public void UploadUserPicture(byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			UploadUserPicture(Token.Instance, pictureData, boundary, onSuccess, onError);
		}

		/// <summary>
		/// Deletes the user’s avatar. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="onSuccess">Called after user profile picture was successfully removed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="UploadUserPicture"/>
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			var url = URL_USER_PICTURE;
			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => DeleteUserPicture(Token.Instance, onSuccess, onError)));
		}

		public void DeleteUserPicture(Action onSuccess, Action<Error> onError)
		{
			DeleteUserPicture(Token.Instance, onSuccess, onError);
		}

		/// <summary>
		/// Checks user age for a particular region. The age requirements depend on the region. Service determines the user location by the IP address.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="dateOfBirth">User's birth date in the `YYYY-MM-DD` format.</param>
		/// <param name="onSuccess">Called after successful check of the user age.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void CheckUserAge(string dateOfBirth, Action<UserCheckAgeResult> onSuccess, Action<Error> onError)
		{
			var request = new UserCheckAgeRequest
			{
				dob = dateOfBirth,
				project_id = XsollaSettings.LoginId
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_CHECK_AGE, request, onSuccess, onError);
		}

		/// <summary>
		/// Returns the user’s email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="onSuccess">Called after user email was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetUserEmail(string token, Action<string> onSuccess, Action<Error> onError)
		{
			Action<UserEmail> successCallback = response => { onSuccess?.Invoke(response.current_email); };
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_GET_EMAIL, WebRequestHeader.AuthHeader(token), successCallback,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetUserEmail(Token.Instance, onSuccess, onError)));
		}

		public void GetUserEmail(Action<string> onSuccess, Action<Error> onError)
		{
			GetUserEmail(Token.Instance, onSuccess, onError);
		}
	}
}
