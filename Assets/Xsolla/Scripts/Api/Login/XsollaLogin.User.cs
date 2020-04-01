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

		public void Registration(string username, string password, string email, Action onSuccess, Action<Error> onError)
		{
			var registrationData = new RegistrationJson(username, password, email);
			
			string proxy = XsollaSettings.UseProxy ? "proxy/registration" : "user";
			string url = GetUrl(URL_USER_REGISTRATION, proxy);

			WebRequestHelper.Instance.PostRequest<RegistrationJson>(url, registrationData, onSuccess, onError, Error.RegistrationErrors);
		}

		public void ResetPassword(string username, Action onSuccess, Action<Error> onError)
		{
			string proxy = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";
			string url = GetUrl(URL_PASSWORD_RESET, proxy, false);
			
			WebRequestHelper.Instance.PostRequest<ResetPassword>(url, new ResetPassword(username), onSuccess, onError, Error.ResetPasswordErrors);
		}

		public void SignInShadowAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
		{
			string url = URL_USER_SHADOW + "?user_id=" + userId + "&platform=" + platform;
			WebRequestHelper.Instance.GetRequest(url, null, (TokenEntity result) => { successCase?.Invoke(result.token); }, failedCase);
		}

		public void SignIn(string username, string password, bool rememberUser, Action onSuccess, Action<Error> onError)
		{
			var loginData = new LoginJson(username, password, rememberUser);
			
			string proxy = XsollaSettings.UseProxy ? "proxy/" : string.Empty;
			string url = GetUrl(URL_USER_SIGNIN, proxy);

			WebRequestHelper.Instance.PostRequest<LoginResponse, LoginJson>(url, loginData, (response) =>
			{
				if (rememberUser) {
					SaveLoginPassword(username, password);
				}
				Token = ParseUtils.ParseToken(response.login_url);
				onSuccess?.Invoke();
			}, onError, Error.LoginErrors);
		}

		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
		{
			List<WebRequestHeader> headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(Token)
			};
			string url = URL_LINKING_CODE_REQUEST + AdditionalUrlParams;
			WebRequestHelper.Instance.PostRequest<LinkingCode>(url, headers, onSuccess, onError);
		}

		public void LinkAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
		{
			string url = URL_LINK_ACCOUNT + "?user_id=" + userId + "&platform=" + platform + "&code=" + confirmationCode;
			WebRequestHelper.Instance.PostRequest(url, null, onSuccess, onError);
		}

		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest<UserInfo>(URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}