using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_REGISTRATION = "https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}&{3}";
		private const string URL_USER_OAUTH_REGISTRATION = "https://login.xsolla.com/api/oauth2/user?response_type=code&client_id={0}&state=xsollatest&redirect_uri=https://login.xsolla.com/api/blank&{1}";
		private const string URL_USER_SIGNIN = "https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}&{3}&{4}";
		private const string URL_USER_OAUTH_SIGNIN = "https://login.xsolla.com/api/oauth2/login/token?client_id={0}&scope=offline&{1}";
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me?{0}";
		private const string URL_USER_PHONE = "https://login.xsolla.com/api/users/me/phone?{0}";
		private const string URL_USER_PICTURE = "https://login.xsolla.com/api/users/me/picture?{0}";
		private const string URL_PASSWORD_RESET = "https://login.xsolla.com/api/{0}?projectId={1}&{2}";
		private const string URL_SEARCH_USER = "https://login.xsolla.com/api/users/search/by_nickname?nickname={0}&offset={1}&limit={2}&{3}";
		private const string URL_USER_PUBLIC_INFO = "https://login.xsolla.com/api/users/{0}/public?{1}";

		/// <summary>
		/// Return saved user info by JWT.
		/// </summary>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_USER_INFO, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(url, headers, onSuccess, onError);
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
			var url = string.Format(URL_USER_INFO, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.PatchRequest(url, info, headers, onSuccess, onError);
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
			string url = default(string);
			
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var proxy = XsollaSettings.UseProxy ? "proxy/registration" : "user";
				url = string.Format(URL_USER_REGISTRATION, proxy, XsollaSettings.LoginId, XsollaSettings.CallbackUrl, AnalyticUrlAddition);
			}
			else/*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				var clientId = XsollaSettings.OAuthClientId;
				url = string.Format(URL_USER_OAUTH_REGISTRATION, clientId, AnalyticUrlAddition);
			}

			WebRequestHelper.Instance.PostRequest<RegistrationJson>(url, registrationData, AnalyticHeaders, onSuccess, onError, Error.RegistrationErrors);
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
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtSignIn(username, password, rememberUser, onSuccess, onError);
			else/*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthSignIn(username, password, onSuccess, onError);
		}

		private void JwtSignIn(string username, string password, bool rememberUser, Action onSuccess, Action<Error> onError)
		{
			var loginData = new LoginJwtJsonRequest(username, password, rememberUser);

			var proxy = XsollaSettings.UseProxy ? "proxy/" : string.Empty;
			var tokenInvalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled ? "with_logout=1" : "with_logout=0";
			var url = string.Format(URL_USER_SIGNIN, proxy, XsollaSettings.LoginId, XsollaSettings.CallbackUrl, tokenInvalidationFlag, AnalyticUrlAddition);

			WebRequestHelper.Instance.PostRequest<LoginJwtJsonResponse, LoginJwtJsonRequest>(url, loginData, AnalyticHeaders, (response) => {
				Token = ParseUtils.ParseToken(response.login_url);
				onSuccess?.Invoke();
			}, onError, Error.LoginErrors);
		}

		private void OAuthSignIn(string username, string password, Action onSuccess, Action<Error> onError)
		{
			var loginData = new LoginOAuthJsonRequest(username, password);
			var url = string.Format(URL_USER_OAUTH_SIGNIN, XsollaSettings.OAuthClientId, AnalyticUrlAddition);

			Action<LoginOAuthJsonResponse> successCallback = response =>
			{
				this.ProcessOAuthResponse(response);
				onSuccess?.Invoke();
			};

			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse, LoginOAuthJsonRequest>(url, loginData, AnalyticHeaders, successCallback, onError, Error.LoginErrors);
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
			var proxy = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";
			var url = string.Format(URL_PASSWORD_RESET, proxy, XsollaSettings.LoginId, AnalyticUrlAddition);

			WebRequestHelper.Instance.PostRequest(url, new ResetPassword(username), AnalyticHeaders, onSuccess, onError, Error.ResetPasswordErrors);
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
			var url = string.Format(URL_SEARCH_USER, nickname, offset, limit, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(url, headers, onSuccess, onError);
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
			var url = string.Format(URL_USER_PUBLIC_INFO, user, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(url, headers, onSuccess, onError);
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
			var url = string.Format(URL_USER_PHONE, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(url, headers,
				onComplete: (UserPhoneNumber number) => onSuccess?.Invoke(number.phone_number),
				onError: onError);
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
			var url = string.Format(URL_USER_PHONE, AnalyticUrlAddition);
			var request = new UserPhoneNumber { phone_number = phoneNumber };
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.PostRequest(url, request, headers, onSuccess, onError);
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
			var url = URL_USER_PHONE.Replace("?", $"/{phoneNumber}?");
			url = string.Format(url, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.DeleteRequest(url, headers, onSuccess, onError);
		}

		/// <summary>
		/// Uploads the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Upload User Picture</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-picture/postusersmepicture"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="pictureData">User profile picture in the binary format.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="DeleteUserPicture"/>
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			var url = string.Format(URL_USER_PICTURE, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token), new WebRequestHeader() { Name = "Content-type", Value = $"multipart/form-data; boundary ={boundary}" });
			WebRequestHelper.Instance.PostUploadRequest(URL_USER_PICTURE, pictureData, headers, onSuccess, onError);
		}
		
		/// <summary>
		/// Deletes the profile picture of the authenticated user by JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete User Picture</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-picture/deleteusersmepicture"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="UploadUserPicture"/>
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			var url = string.Format(URL_USER_PICTURE, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.DeleteRequest(url, headers, onSuccess, onError);
		}
	}
}