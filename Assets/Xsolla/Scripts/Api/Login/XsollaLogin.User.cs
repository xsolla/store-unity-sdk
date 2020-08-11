using System;
using System.Collections.Generic;
using System.Text;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_REGISTRATION = "https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}";
		private const string URL_USER_SIGNIN = "https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}";
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me";
		private const string URL_USER_PHONE = "https://login.xsolla.com/api/users/me/phone";
		private const string URL_USER_PICTURE = "https://login.xsolla.com/api/users/me/picture";
		private const string URL_PASSWORD_RESET = "https://login.xsolla.com/api/{0}?projectId={1}";
		private const string URL_SEARCH_USER = "https://login.xsolla.com/api/users/search/by_nickname?nickname={0}&offset={1}&limit={2}";
		private const string URL_USER_PUBLIC_INFO = "https://login.xsolla.com/api/users/{0}/public";

		private string GetUrl(string url, string proxy, bool useCallback = true)
		{
			StringBuilder urlBuilder;
			if (useCallback)
				urlBuilder = new StringBuilder(string.Format(url, proxy, XsollaSettings.LoginId, XsollaSettings.CallbackUrl));
			else
				urlBuilder = new StringBuilder(string.Format(url, proxy, XsollaSettings.LoginId));
			urlBuilder.Append(AdditionalUrlParams);
			return urlBuilder.ToString();
		}

		/// <summary>
		/// Return saved user info by JWT.
		/// </summary>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
		
		/// <summary>
		/// Updates the details of the authenticated user by JWT.
		/// </summary>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="info">User information.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.PatchRequest(URL_USER_INFO, info, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// User registration method.
		/// </summary>
		/// <remarks> Swagger method name:<c>Register</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-register"/>
		/// <param name="username">User name.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email for verification.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignIn"/>
		/// <seealso cref="ResetPassword"/>
		public void Registration(string username, string password, string email, Action onSuccess, Action<Error> onError = null)
		{
			var registrationData = new RegistrationJson(username, password, email);
			
			string proxy = XsollaSettings.UseProxy ? "proxy/registration" : "user";
			string url = GetUrl(URL_USER_REGISTRATION, proxy);

			WebRequestHelper.Instance.PostRequest<RegistrationJson>(url, registrationData, onSuccess, onError, Error.RegistrationErrors);
		}

		/// <summary>
		/// Perform Base Authorization.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth by Username and Password</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/auth-by-username-and-password"/>
		/// <param name="username">User name.</param>
		/// <param name="password">User password.</param>
		/// <param name="rememberUser">Save user credentionals?</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignInConsoleAccount"/>
		/// <seealso cref="Registration"/>
		/// <seealso cref="ResetPassword"/>
		public void SignIn(string username, string password, bool rememberUser, Action onSuccess, Action<Error> onError = null)
		{
			var loginData = new LoginJson(username, password, rememberUser);

			string proxy = XsollaSettings.UseProxy ? "proxy/" : string.Empty;
			string url = GetUrl(URL_USER_SIGNIN, proxy);

			var tokenInvalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled ? "&with_logout=1" : "&with_logout=0";
			url += tokenInvalidationFlag;

			WebRequestHelper.Instance.PostRequest<LoginResponse, LoginJson>(url, loginData, (response) => {
				Token = ParseUtils.ParseToken(response.login_url);
				onSuccess?.Invoke();
			}, onError, Error.LoginErrors);
		}

		/// <summary>
		/// Allow user to reset password.
		/// </summary>
		/// <remarks> Swagger method name:<c>Reset password</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/general/reset-password"/>
		/// <param name="username">User name.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="Registration"/>
		/// <seealso cref="SignIn"/>
		public void ResetPassword(string username, Action onSuccess, Action<Error> onError = null)
		{
			string proxy = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";
			string url = GetUrl(URL_PASSWORD_RESET, proxy, false);
			
			WebRequestHelper.Instance.PostRequest(url, new ResetPassword(username), onSuccess, onError, Error.ResetPasswordErrors);
		}

		/// <summary>
		/// Search users by nickname in the same Login as current user.
		/// NOTE: User can search only 1 time per second.
		/// </summary>
		/// <remarks> Swagger method name:<c>Search user by nickname</c>.</remarks>
		/// <see cref="https://go-xsolla-login.doc.srv.loc/login-api/users/get-users-search-by-nickname"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="nickname">User's nickname.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SearchUsers(string token, string nickname, uint offset, uint limit, Action<FoundUsers> onSuccess, Action<Error> onError = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_SEARCH_USER, nickname, offset, limit));
			urlBuilder.Append(AdditionalUrlParams);
			var url = urlBuilder.ToString();
			WebRequestHelper.Instance.GetRequest(url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Returns public user info by user ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get public user's profile</c>.</remarks>
		/// <see cref="https://go-xsolla-login.doc.srv.loc/login-api/users/get-public-user-profile"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="user">The Xsolla Login user ID.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_USER_PUBLIC_INFO, user));
			urlBuilder.Append(AdditionalUrlParams);
			var url = urlBuilder.ToString();
			WebRequestHelper.Instance.GetRequest(url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Gets the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-phone-number/getusersmephone"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="ChangeUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public void GetUserPhoneNumber(string token, Action<string> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest(URL_USER_PHONE, WebRequestHeader.AuthHeader(token), 
				(UserPhoneNumber number) => onSuccess?.Invoke(number.phone_number), onError);
		}

		/// <summary>
		/// Updates the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-phone-number/postusersmephone"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="phoneNumber">User's phone number.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var request = new UserPhoneNumber {phone_number = phoneNumber};
			var headers = new List<WebRequestHeader> {WebRequestHeader.AuthHeader(token)};
			WebRequestHelper.Instance.PostRequest(URL_USER_PHONE, request, headers, onSuccess, onError);
		}
		
		/// <summary>
		/// Deletes the phone number of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-phone-number/deleteusersmephonephonenumber"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="phoneNumber">User's phone number.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="ChangeUserPhoneNumber"/>
		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.DeleteRequest(URL_USER_PHONE + $"/{phoneNumber}", WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Uploads the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-picture/postusersmepicture"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="pathToPicture">Path to user profile picture in the binary format.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="DeleteUserPicture"/>
		public void UploadUserPicture(string token, string pathToPicture, Action<string> onSuccess, Action<Error> onError)
		{
			var headers = new List<WebRequestHeader> { WebRequestHeader.AuthHeader(token) };
			WebRequestHelper.Instance.PostUploadRequest(URL_USER_PICTURE, pathToPicture, headers, 
				(UserPictureUploadResponse response) => onSuccess?.Invoke(response.picture), onError);
		}
		
		/// <summary>
		/// Deletes the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Phone Number</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-picture/deleteusersmepicture"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="UploadUserPicture"/>
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.DeleteRequest(URL_USER_PICTURE, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}