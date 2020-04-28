using System;
using UnityEngine;
using System.Text;
using Xsolla.Core;
using System.Collections.Generic;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_REGISTRATION = "https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}";
		private const string URL_USER_SIGNIN = "https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}";
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me";
		private const string URL_PASSWORD_RESET = "https://login.xsolla.com/api/{0}?projectId={1}";

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
			WebRequestHelper.Instance.GetRequest<UserInfo>(URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
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

			WebRequestHelper.Instance.PostRequest<LoginResponse, LoginJson>(url, loginData, (response) => {
				if (rememberUser) {
					SaveLoginPassword(username, password);
				}
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
			
			WebRequestHelper.Instance.PostRequest<ResetPassword>(url, new ResetPassword(username), onSuccess, onError, Error.ResetPasswordErrors);
		}
	}
}