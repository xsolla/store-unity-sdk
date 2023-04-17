using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_USER_REGISTRATION = "https://login.xsolla.com/api/oauth2/user?response_type=code&client_id={0}&state={1}&redirect_uri={2}{3}";
		private const string URL_USER_SIGNIN = "https://login.xsolla.com/api/oauth2/login/token?client_id={0}&scope=offline&redirect_uri={1}";
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me";
		private const string URL_PASSWORD_RESET = "https://login.xsolla.com/api/password/reset/request?projectId={0}&login_url={1}{2}";
		private const string URL_RESEND_CONFIRMATION_LINK = "https://login.xsolla.com/api/oauth2/user/resend_confirmation_link?client_id={0}&state={1}&redirect_uri={2}{3}";
		private const string URL_USER_SOCIAL_NETWORK_TOKEN_AUTH = "https://login.xsolla.com/api/oauth2/social/{0}/login_with_token?client_id={1}&response_type=code&redirect_uri={2}&state={3}&scope=offline";
		private const string URL_GET_ACCESS_TOKEN = "{0}/login";
		private const string URL_OAUTH_LOGOUT = "https://login.xsolla.com/api/oauth2/logout?sessions={0}";

		private const string URL_START_AUTH_BY_EMAIL = "https://login.xsolla.com/api/oauth2/login/email/request?response_type=code&client_id={0}&scope=offline&state={1}&redirect_uri={2}{3}";
		private const string URL_COMPLETE_AUTH_BY_EMAIL = "https://login.xsolla.com/api/oauth2/login/email/confirm?client_id={0}";
		private const string URL_START_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/oauth2/login/phone/request?response_type=code&client_id={0}&scope=offline&state={1}&redirect_uri={2}";
		private const string URL_COMPLETE_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/oauth2/login/phone/confirm?client_id={0}";

		private const string URL_XSOLLA_LOGIN_WIDGET = "https://login-widget.xsolla.com/latest/?projectId={0}&login_url={1}";

		/// <summary>
		/// Returns user details.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="onSuccess">Called after successful user details were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Creates a new user account in the application and sends a sign-up confirmation email to the specified email address. To complete registration, the user must follow the link from the email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email address.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		/// Required if there are several URIs.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="acceptConsent">Whether the user gave consent to processing of their personal data.</param>
		/// <param name="fields">Parameters used for extended registration form. To use this feature, please contact your Account Manager.</param>
		/// <param name="promoEmailAgreement">User consent to receive the newsletter.</param>
		/// <param name="locale">Defines localization of the email the user receives.<br/>
		/// The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <param name="onSuccess">Called after successful user registration. Account confirmation message will be sent to the specified email address.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="SignIn"/>
		/// <seealso href="ResetPassword"/>
		/// <seealso href="ResendConfirmationLink"/>
		public void Register(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, string locale = null, Action<int> onSuccess = null, Action<Error> onError = null)
		{
			var registrationData = new RegistrationJson(username, password, email, acceptConsent, fields, promoEmailAgreement);
			var url = GetRegistrationUrl(oauthState, redirectUri, locale);
			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, registrationData, onSuccess, onError, ErrorCheckType.RegistrationErrors);
		}

		public void Register(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, string locale = null, Action onSuccess = null, Action<Error> onError = null)
		{
			Action<int> onSuccessRegistration = _ => onSuccess?.Invoke();
			Register(username, password, email, redirectUri, oauthState, payload, acceptConsent, promoEmailAgreement, fields, locale, onSuccessRegistration, onError);
		}

		public void Register(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, string locale = null, Action<LoginUrlResponse> onSuccess = null, Action<Error> onError = null)
		{
			var registrationData = new RegistrationJson(username, password, email, acceptConsent, fields, promoEmailAgreement);
			var url = GetRegistrationUrl(oauthState, redirectUri, locale);
			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, registrationData, onSuccess, onError, ErrorCheckType.RegistrationErrors);
		}

		private string GetRegistrationUrl(string oauthState = null, string redirectUri = null, string locale = null)
		{
			var clientIdParam = XsollaSettings.OAuthClientId;
			var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
			var redirectUriParam = RedirectUtils.GetRedirectUrl(redirectUri);
			var localeParam = (!string.IsNullOrEmpty(locale)) ? $"&locale={locale}" : "";
			return string.Format(URL_USER_REGISTRATION, clientIdParam, stateParam, redirectUriParam, localeParam);
		}

		/// <summary>
		/// Authenticates the user by the username/email and password specified via the authentication interface.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username or email address.</param>
		/// <param name="password">User password.</param>
		/// <param name="rememberMe">Whether the user agrees to save the authentication data.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		/// Required if there are several URIs.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="onSuccess">Called after successful user authentication. Authentication data including the JWT will be received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="SignInConsoleAccount"/>
		/// <seealso href="Register"/>
		/// <seealso href="ResetPassword"/>
		/// <seealso href="ResendConfirmationLink"/>
		public void SignIn(string username, string password, bool rememberMe, string redirectUri = null, string payload = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var loginData = new LoginRequest(username, password);
			var redirectUriParam = RedirectUtils.GetRedirectUrl(redirectUri);
			var url = string.Format(URL_USER_SIGNIN, XsollaSettings.OAuthClientId, redirectUriParam);

			Action<LoginOAuthJsonResponse> successCallback = response =>
			{
				ProcessOAuthResponse(response, onSuccess);
			};

			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse, LoginRequest>(SdkType.Login, url, loginData, successCallback, onError, ErrorCheckType.LoginErrors);
		}

		/// <summary>
		/// Starts user authentication and sends an email with a one-time code and a link to the specified email address (if login via magic link is configured for the Login project)
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="email">User email address.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the email or not.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="locale">Defines localization of the email the user receives.
		/// The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <param name="onSuccess">Called after successful email authentication start.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void StartAuthByEmail(string email, string linkUrl, bool? sendLink, string oauthState = null, string locale = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var data = new StartAuthByEmailRequest(email, linkUrl, sendLink);
			var state = oauthState ?? DEFAULT_OAUTH_STATE;
			var redirectParam = RedirectUtils.GetRedirectUrl();
			var localeParam = (!string.IsNullOrEmpty(locale)) ? $"&locale={locale}" : "";
			var url = string.Format(URL_START_AUTH_BY_EMAIL, XsollaSettings.OAuthClientId, state, redirectParam, localeParam);

			WebRequestHelper.Instance.PostRequest<StartAuthByEmailResponse, StartAuthByEmailRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				ErrorCheckType.LoginErrors);
		}

		public void StartAuthByEmail(string email, string linkUrl, bool? sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			StartAuthByEmail(email:email, linkUrl:linkUrl, sendLink:sendLink, oauthState:null, onSuccess:onSuccess, onError:onError);
		}

		/// <summary>
		/// Completes authentication after the user enters a one-time code or follows a link received in an email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="email">User email address.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">Identifier of the confirmation code.</param>
		/// <param name="onSuccess">Called after successful email authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByEmailRequest(email, confirmationCode, operationId);
			var url = string.Format(URL_COMPLETE_AUTH_BY_EMAIL, XsollaSettings.OAuthClientId);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, CompleteAuthByEmailRequest>(
				SdkType.Login,
				url,
				data,
				response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out var parsedCode))
						ExchangeCodeToToken(parsedCode, token => onSuccess?.Invoke(token), onError);
					else
						onError?.Invoke(Error.UnknownError);
				},
				onError,
				ErrorCheckType.LoginErrors);
		}

		/// <summary>
		/// Starts user authentication and sends an SMS with a one-time code and a link to the specified phone number (if login via magic link is configured for the Login project).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the SMS or not.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="onSuccess">Called after successful phone number authentication start.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, string oauthState, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new StartAuthByPhoneNumberRequest(phoneNumber, linkUrl, sendLink);
			var state = oauthState ?? DEFAULT_OAUTH_STATE;
			var redirectParam = RedirectUtils.GetRedirectUrl();
			var url = string.Format(URL_START_AUTH_BY_PHONE_NUMBER, XsollaSettings.OAuthClientId, state, redirectParam);

			WebRequestHelper.Instance.PostRequest<StartAuthByPhoneNumberResponse, StartAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				ErrorCheckType.LoginErrors);
		}

		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			StartAuthByPhoneNumber(phoneNumber:phoneNumber, linkUrl:linkUrl, sendLink:sendLink, oauthState:null, onSuccess:onSuccess, onError:onError);
		}

		/// <summary>
		/// Completes authentication after the user enters a one-time code or follows a link received by SMS.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">Identifier of the confirmation code.</param>
		/// <param name="onSuccess">Called after successful phone number authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByPhoneNumberRequest(phoneNumber, confirmationCode, operationId);
			var url = string.Format(URL_COMPLETE_AUTH_BY_PHONE_NUMBER, XsollaSettings.OAuthClientId);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, CompleteAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				data,
				response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out var parsedCode))
						ExchangeCodeToToken(parsedCode, token => onSuccess?.Invoke(token), onError);
					else
						onError?.Invoke(Error.UnknownError);
				},
				onError,
				ErrorCheckType.LoginErrors);
		}

		/// <summary>
		/// Resets the user’s current password and sends an email to change the password to the email address specified during sign-up.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="email">Email to send the password change verification message to.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		/// Required if there are several URIs.</param>
		/// <param name="onSuccess">Called after successful user password reset.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the email the user receives.
		/// The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <seealso href="Register"/>
		/// <seealso href="SignIn"/>
		/// <seealso href="ResendConfirmationLink"/>
		public void ResetPassword(string email, string redirectUri = null, string locale = null, Action onSuccess = null, Action<Error> onError = null)
		{
			var projectIdParam = XsollaSettings.LoginId;
			var loginUrlParam = RedirectUtils.GetRedirectUrl(redirectUri);
			var localeParam = (!string.IsNullOrEmpty(locale)) ? $"&locale={locale}" : "";
			var url = string.Format(URL_PASSWORD_RESET, projectIdParam, loginUrlParam, localeParam);

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, new ResetPassword(email), onSuccess, onError, ErrorCheckType.ResetPasswordErrors);
		}

		/// <summary>
		/// Resends a sign-up confirmation email to the specified email address. To complete registration, the user must follow the link from the email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username or user email address.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		/// Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="locale">Defines localization of the email user receives.
		/// The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <param name="onSuccess">Called after successful sending of the request.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="Register"/>
		/// <seealso href="SignIn"/>
		/// <seealso href="ResetPassword"/>
		public void ResendConfirmationLink(string username, string redirectUri = null, string state = null, string payload = null, string locale = null, Action onSuccess = null, Action<Error> onError = null)
		{
			var stateParam = state ?? DEFAULT_OAUTH_STATE;
			var redirectUriParam = RedirectUtils.GetRedirectUrl(redirectUri);
			var localeParam = (!string.IsNullOrEmpty(locale)) ? $"&locale={locale}" : "";
			var url = string.Format(URL_RESEND_CONFIRMATION_LINK, XsollaSettings.OAuthClientId, stateParam, redirectUriParam, localeParam);

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, new ResendConfirmationLinkRequest(username), onSuccess, onError);
		}

		/// <summary>
		/// Authenticates the user with the access token using social network credentials.
		/// </summary>
		/// <param name="accessToken">Access token received from a social network.</param>
		/// <param name="accessTokenSecret">Parameter `oauth_token_secret` received from the authorization request. Required for Twitter only.</param>
		/// <param name="openId">Parameter `openid` received from the social network. Required for WeChat only.</param>
		/// <param name="providerName">Name of the social network connected to Login in Publisher Account. Can be `facebook`, `google`, `wechat`, or `qq_mobile`.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="onSuccess">Called after successful user authentication on the specified platform.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void AuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string providerName, string payload, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var oauthState = state ?? DEFAULT_OAUTH_STATE;
			var redirectParam = RedirectUtils.GetRedirectUrl();
			var url = string.Format(URL_USER_SOCIAL_NETWORK_TOKEN_AUTH, providerName, XsollaSettings.OAuthClientId, redirectParam, oauthState);

			var requestData = new SocialNetworkAccessTokenRequest
			{
				access_token = accessToken,
				access_token_secret = accessTokenSecret,
				openId = openId
			};

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, requestData, onSuccess, onError, ErrorCheckType.LoginErrors);
		}


		/// <summary>
		/// Authenticates the user with Xsolla Login widget.
		/// </summary>
		/// <param name="onSuccess">Called after successful authentication.</param>
		/// <param name="onCancel">Called after browser closing by user.</param>
		public void AuthWithXsollaWidget(Action<string> onSuccess, Action onCancel = null)
		{
			var url = string.Format(URL_XSOLLA_LOGIN_WIDGET, XsollaSettings.LoginId, RedirectUtils.GetRedirectUrl());
			BrowserHelper.Instance.Open(url);

			var browser = BrowserHelper.Instance.InAppBrowser;

			void onBrowserClose(bool isManually)
			{
				onCancel?.Invoke();
				browser.CloseEvent -= onBrowserClose;
				browser.UrlChangeEvent -= onBrowserUrlChange;
			}

			void onBrowserUrlChange(string newUrl)
			{
				if (ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out var token))
				{
					browser.CloseEvent -= onBrowserClose;
					browser.UrlChangeEvent -= onBrowserUrlChange;
					BrowserHelper.Instance.Close();
					onSuccess?.Invoke(token);
				}
			}

			browser.CloseEvent += onBrowserClose;
			browser.UrlChangeEvent += onBrowserUrlChange;
		}

		/// <summary>
		/// Logs the user out and deletes the user session according to the value of the sessions parameter (OAuth2.0 only).
		/// </summary>
		/// <param name="token">User authorization token.</param>
		/// <param name="sessions">Shows how the user is logged out and how the user session is deleted. Can be `sso` or `all` (default). Leave empty to use the default value.</param>
		/// <param name="onSuccess">Called after successful user logout.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void OAuthLogout(string token, OAuthLogoutType sessions, Action onSuccess, Action<Error> onError = null)
		{
			var logoutTypeFlag = sessions.ToString().ToLowerInvariant();
			var url = string.Format(URL_OAUTH_LOGOUT, logoutTypeFlag);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}
