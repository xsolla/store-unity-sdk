using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Login
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
		/// Updates the details of the authenticated user by JWT.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/api/login/operation/update-user-details/"/>
		/// <remarks> Swagger method name:<c>Update User Details</c>.</remarks>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="info">User information.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.PatchRequest(SdkType.Login, URL_USER_INFO, info, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Searches users by the nickname parameter and gets a list of them. Search can be performed instantly when the user starts entering the search parameter.
		/// NOTE: User can search only 1 time per second.
		/// </summary>
		/// <remarks> Swagger method name:<c>Search Users by Nickname</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-friends/search-users-by-nickname"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="nickname">The search string that may contain: Nickname only, Tag only, Nickname and tag together</param>
		/// <param name="offset">Number of the elements from which the list is generated.</param>
		/// <param name="limit">Maximum number of users that are returned at a time.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SearchUsers(string token, string nickname, uint offset, uint limit, Action<FoundUsers> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_SEARCH_USER, nickname, offset, limit);
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Gets the user information from their public profile by the user ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User Public Profile</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-friends/get-user-public-profile"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="userId">The Xsolla Login user ID.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetPublicInfo(string token, string userId, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_USER_PUBLIC_INFO, userId);
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Gets the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/get-user-phone-number/"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="ChangeUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public void GetUserPhoneNumber(string token, Action<string> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_PHONE, WebRequestHeader.AuthHeader(token),
				onComplete: (UserPhoneNumber number) => onSuccess?.Invoke(number.phone_number),
				onError: onError);
		}

		/// <summary>
		/// Updates the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/update-user-phone-number/"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="phoneNumber">Updated user phone number according to national conventions.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var request = new UserPhoneNumber
			{
				phone_number = phoneNumber
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_PHONE, request, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Deletes the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/delete-user-phone-number/"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="phoneNumber">User phone number according to national conventions.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="ChangeUserPhoneNumber"/>
		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var url = $"{URL_USER_PHONE}/{phoneNumber}";
			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Uploads the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Upload User Picture</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/upload-user-picture/"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="pictureData">User profile picture in the binary format.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="DeleteUserPicture"/>
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			var headers = new List<WebRequestHeader>(){
				WebRequestHeader.AuthHeader(token),
				new WebRequestHeader(){
					Name = "Content-type",
					Value = $"multipart/form-data; boundary ={boundary}"
				}
			};
			WebRequestHelper.Instance.PostUploadRequest(SdkType.Login, URL_USER_PICTURE, pictureData, headers, onSuccess, onError);
		}

		/// <summary>
		/// Deletes the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Picture</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/delete-user-picture/"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="UploadUserPicture"/>
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			var url = URL_USER_PICTURE;
			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Checks user’s age for a particular region. The age requirements depend on the region. Service determines the user’s location by the IP address.
		/// </summary>
		/// <remarks> Swagger method name:<c>Check User's Age</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-profile/check-users-age"/>
		/// <param name="dateOfBirth">User’s birth date in the YYYY-MM-DD format.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
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
		/// Gets the email of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User Email</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-profile/get-user-email"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserEmail(string token, Action<string> onSuccess, Action<Error> onError)
		{
			Action<UserEmail> successCallback = response => { onSuccess?.Invoke(response.current_email); };
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_GET_EMAIL, WebRequestHeader.AuthHeader(token), successCallback, onError);
		}
	}
}
