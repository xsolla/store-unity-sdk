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
		private const string URL_LINKING_CODE_REQUEST = "https://login.xsolla.com/api/users/account/code";
		private const string URL_USER_SHADOW = "https://livedemo.xsolla.com/sdk/shadow_account/auth";
		private const string URL_LINK_ACCOUNT = "https://livedemo.xsolla.com/sdk/shadow_account/link";

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
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest<UserInfo>(URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		#region Basic authorization
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
		public void Registration(string username, string password, string email, Action onSuccess, Action<Error> onError)
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
		/// <seealso cref="SignInShadowAccount"/>
		/// <seealso cref="Registration"/>
		/// <seealso cref="ResetPassword"/>
		public void SignIn(string username, string password, bool rememberUser, Action onSuccess, Action<Error> onError)
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
		public void ResetPassword(string username, Action onSuccess, Action<Error> onError)
		{
			string proxy = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";
			string url = GetUrl(URL_PASSWORD_RESET, proxy, false);
			
			WebRequestHelper.Instance.PostRequest<ResetPassword>(url, new ResetPassword(username), onSuccess, onError, Error.ResetPasswordErrors);
		}
		#endregion

		#region Account Linking
		/// <summary>
		/// This method used for auth users in the Xsolla Login,
		/// who plays on the consoles and other platforms
		/// where Xsolla Login is not used. You must implements it
		/// on the your server side.
		/// Your integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Request credentionals</term>
		///						<description>before write any code, contact with support by email:<see cref="support@xsolla.com"/>
		///						and request <c>ClientID</c> + <c>ClientSecret</c>.
		///						</description>
		///					</item>
		///					<item>
		///						<term>Implement method: </term>
		///						<description>
		///							<see cref="https://developers.xsolla.com/login-api/oauth-20/generate-user-jwt"/>
		///							with application/x-www-form-urlencoded payload parameters:
		///							<list type="bullet">
		///								<item>
		///									<description>client_id=YOUR_CLIENT_ID</description>
		///								</item>
		///								<item>
		///									<description>client_secret=YOUR_CLIENT_SECRET</description>
		///								</item>
		///								<item>
		///									<description>grant_type=client_credentials</description>
		///								</item>
		///							</list>
		///						</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		///		<item>
		///			<term>Implement auth method</term>
		///			<description>
		///				<see cref="https://developers.xsolla.com/login-api/jwt/auth-by-custom-id"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Query parameters</term>
		///						<description><c>?publisher_project_id=XsollaSettings.StoreProjectId</c></description>
		///					</item>
		///					<item>
		///						<term>Headers</term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///					<item>
		///					<term>Body</term>
		///					<description>see documentation.</description>
		///					</item>
		///				</list>
		///				
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="successCase">Success operation callback.</param>
		/// <param name="failedCase">Failed operation callback.</param>
		public void SignInShadowAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
		{
			string url = URL_USER_SHADOW + "?user_id=" + userId + "&platform=" + platform;
			WebRequestHelper.Instance.GetRequest(url, null, (TokenEntity result) => { successCase?.Invoke(result.token); }, failedCase);
		}

		/// <summary>
		/// Request code from unified account to link publishing platform account.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create Code for Linking Accounts</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/users/create-code-for-linking-accounts"/>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignInShadowAccount"/>
		/// <seealso cref="LinkAccount"/>
		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
		{
			List<WebRequestHeader> headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(Token)
			};
			string url = URL_LINKING_CODE_REQUEST + "?" + AdditionalUrlParams.TrimStart('&');
			WebRequestHelper.Instance.PostRequest<LinkingCode>(url, headers, onSuccess, onError);
		}

		/// <summary>
		/// This method used for auth users in the Xsolla Login,
		/// who plays on the consoles and other platforms
		/// where Xsolla Login is not used. You must implements it
		/// on the your server side.
		/// Your integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Request credentionals</term>
		///						<description>before write any code, contact with support by email:<see cref="support@xsolla.com"/>
		///						and request <c>ClientID</c> + <c>ClientSecret</c>.
		///						</description>
		///					</item>
		///					<item>
		///						<term>Implement method: </term>
		///						<description>
		///							<see cref="https://developers.xsolla.com/login-api/oauth-20/generate-user-jwt"/>
		///							with application/x-www-form-urlencoded payload parameters:
		///							<list type="bullet">
		///								<item>
		///									<description>client_id=YOUR_CLIENT_ID</description>
		///								</item>
		///								<item>
		///									<description>client_secret=YOUR_CLIENT_SECRET</description>
		///								</item>
		///								<item>
		///									<description>grant_type=client_credentials</description>
		///								</item>
		///							</list>
		///						</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		///		<item>
		///			<term>Implement linking accounts method</term>
		///			<description>
		///				<see cref="https://developers.xsolla.com/login-api/users/link-accounts-by-code"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Headers</term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///					<item>
		///					<term>Body</term>
		///					<description>see documentation.</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="confirmationCode">Code, taken from unified account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignInShadowAccount"/>
		/// <seealso cref="RequestLinkingCode"/>
		public void LinkAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
		{
			string url = URL_LINK_ACCOUNT + "?user_id=" + userId + "&platform=" + platform + "&code=" + confirmationCode;
			WebRequestHelper.Instance.PostRequest(url, null, onSuccess, onError);
		}
		#endregion
	}
}