using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_JWT_USER_REGISTRATION = "https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}{3}";
		private const string URL_OAUTH_USER_REGISTRATION = "https://login.xsolla.com/api/oauth2/user?response_type=code&client_id={0}&state={1}&redirect_uri={2}";
		private const string URL_JWT_USER_SIGNIN = "https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}{3}&with_logout={4}";
		private const string URL_OAUTH_USER_SIGNIN = "https://login.xsolla.com/api/oauth2/login/token?client_id={0}&scope=offline{1}";
		private const string URL_USER_INFO = "https://login.xsolla.com/api/users/me";
		private const string URL_PASSWORD_RESET = "https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}";
		private const string URL_JWT_RESEND_CONFIRMATION_LINK = "https://login.xsolla.com/api/user/resend_confirmation_link?projectId={0}&login_url={1}{2}";
		private const string URL_OAUTH_RESEND_CONFIRMATION_LINK = "https://login.xsolla.com/api/oauth2/user/resend_confirmation_link?client_id={0}&state={1}&redirect_uri={2}";
		private const string URL_JWT_USER_SOCIAL_NETWORK_TOKEN_AUTH = "https://login.xsolla.com/api/social/{0}/login_with_token?projectId={1}&payload={2}&with_logout={3}";
		private const string URL_OAUTH_USER_SOCIAL_NETWORK_TOKEN_AUTH = "https://login.xsolla.com/api/oauth2/social/{0}/login_with_token?client_id={1}&response_type=code&redirect_uri={2}&state={3}&scope=offline";
		private const string URL_GET_ACCESS_TOKEN = "{0}/login";
		private const string URL_OAUTH_LOGOUT = "https://login.xsolla.com/api/oauth2/logout?sessions={0}";

		private const string URL_JWT_START_AUTH_BY_EMAIL = "https://login.xsolla.com/api/login/email/request?projectId={0}&login_url={1}&with_logout={2}&payload={3}";
		private const string URL_OAUTH_START_AUTH_BY_EMAIL = "https://login.xsolla.com/api/oauth2/login/email/request?response_type=code&client_id={0}&scope=offline&state={1}&redirect_uri={2}";
		private const string URL_JWT_COMPLETE_AUTH_BY_EMAIL = "https://login.xsolla.com/api/login/email/confirm?projectId={0}";
		private const string URL_OAUTH_COMPLETE_AUTH_BY_EMAIL = "https://login.xsolla.com/api/oauth2/login/email/confirm?client_id={0}";
		private const string URL_JWT_START_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/login/phone/request?projectId={0}&login_url={1}&with_logout={2}&payload={3}";
		private const string URL_OAUTH_START_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/oauth2/login/phone/request?response_type=code&client_id={0}&scope=offline&state={1}&redirect_uri={2}";
		private const string URL_JWT_COMPLETE_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/login/phone/confirm?projectId={0}";
		private const string URL_OAUTH_COMPLETE_AUTH_BY_PHONE_NUMBER = "https://login.xsolla.com/api/oauth2/login/phone/confirm?client_id={0}";

		/// <summary>
		/// Gets details of the user authenticated by the JWT.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User Details</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/get-user-details/"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_USER_INFO, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Creates a new user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Register New User</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-register"/>
		/// <see cref="https://developers.xsolla.com/login-api/auth/oauth-20/oauth-20-register-new-user"/>
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email address.</param>
		/// <param name="redirectUri">URL to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the Callback URL specified in the URL block of Publisher Account.
		/// To find it, go to Login > your Login project > General settings. Required if there are several Callback URLs.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 symbols. Used only for OAuth2.0 auth.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		/// <param name="acceptConsent">Whether the user gave consent to processing of their personal data.</param>
		/// <param name="fields">Parameters used for extended registration form. To use this feature, please contact your Account Manager.</param>
		/// <param name="promoEmailAgreement">User consent to receive the newsletter.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignIn"/>
		/// <seealso cref="ResetPassword"/>
		public void Registration(string username, string password, string email, string redirectUri = null, string state = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action<int> onSuccess = null, Action<Error> onError = null)
		{
			var registrationData = new RegistrationJson(username, password, email, acceptConsent, fields, promoEmailAgreement);
			var url = GetRegistrationUrl(state, payload, redirectUri);
			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, registrationData, onSuccess, onError, Error.RegistrationErrors);
		}

		public void Registration(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action onSuccess = null, Action<Error> onError = null)
		{
			Action<int> onSuccessRegistration = _ => onSuccess?.Invoke();
			Registration(username, password, email, redirectUri, oauthState, payload, acceptConsent, promoEmailAgreement, fields, onSuccessRegistration, onError);
		}

		public void Registration(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action<LoginUrlResponse> onSuccess = null, Action<Error> onError = null)
		{
			var registrationData = new RegistrationJson(username, password, email, acceptConsent, fields, promoEmailAgreement);
			var url = GetRegistrationUrl(oauthState, payload, redirectUri);
			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, registrationData, onSuccess, onError, Error.RegistrationErrors);
		}

		private string GetRegistrationUrl(string oauthState = null, string payload = null, string redirectUri = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var proxyParam = XsollaSettings.UseProxy ? "proxy/registration" : "user";
				var projectIdParam = XsollaSettings.LoginId;
				var loginUrlParam = (!string.IsNullOrEmpty(redirectUri)) ? redirectUri : XsollaSettings.CallbackUrl;
				var payloadParam = (!string.IsNullOrEmpty(payload)) ? $"&payload={payload}" : "";
				return string.Format(URL_JWT_USER_REGISTRATION, proxyParam, projectIdParam, loginUrlParam, payloadParam);
			}
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				var clientIdParam = XsollaSettings.OAuthClientId;
				var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
				var redirectUriParam = (!string.IsNullOrEmpty(redirectUri)) ? redirectUri : DEFAULT_REDIRECT_URI;
				return string.Format(URL_OAUTH_USER_REGISTRATION, clientIdParam, stateParam, redirectUriParam);
			}
		}

		/// <summary>
		/// Authenticates the user by the username/email and password specified.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth by Username and Password</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/auth-by-username-and-password"/>
		/// <see cref="https://developers.xsolla.com/login-api/auth/oauth-20/oauth-20-auth-by-username-and-password"/>
		/// <param name="username">Username or email address.</param>
		/// <param name="password">User password.</param>
		/// <param name="rememberMe">Whether the user agrees to save the authentication data. Default is false. Used only for JWT auth.</param>
		/// <param name="redirectUri">URL to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the Callback URL specified in the URL block of Publisher Account.
		/// To find it, go to Login > your Login project > General settings. Required if there are several Callback URLs.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="SignInConsoleAccount"/>
		/// <seealso cref="Registration"/>
		/// <seealso cref="ResetPassword"/>
		public void SignIn(string username, string password, bool rememberMe, string redirectUri = null, string payload = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtSignIn(username, password, rememberMe, redirectUri, payload, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthSignIn(username, password, redirectUri, onSuccess, onError);
		}

		private void JwtSignIn(string username, string password, bool rememberUser, string redirectUri, string payload, Action<string> onSuccess, Action<Error> onError)
		{
			var loginData = new LoginRequest(username, password, rememberUser);

			var proxyParam = XsollaSettings.UseProxy ? "proxy/" : string.Empty;
			var projectIdParam = XsollaSettings.LoginId;
			var loginUrlParam = (!string.IsNullOrEmpty(redirectUri)) ? redirectUri : XsollaSettings.CallbackUrl;
			var payloadParam = (!string.IsNullOrEmpty(payload)) ? $"&payload={payload}" : "";
			var withLogoutParam = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			var url = string.Format(URL_JWT_USER_SIGNIN, proxyParam, projectIdParam, loginUrlParam, payloadParam, withLogoutParam);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, LoginRequest>(SdkType.Login, url, loginData, (response) =>
			{
				var parsedToken = ParseUtils.ParseToken(response.login_url);
				Token.Instance = Token.Create(parsedToken);
				onSuccess?.Invoke(Token.Instance);
			}, onError, Error.LoginErrors);
		}

		private void OAuthSignIn(string username, string password, string redirectUri, Action<string> onSuccess, Action<Error> onError)
		{
			var loginData = new LoginRequest(username, password);
			var redirectUriParam = (!string.IsNullOrEmpty(redirectUri)) ? $"&redirect_uri={redirectUri}" : string.Empty;
			var url = string.Format(URL_OAUTH_USER_SIGNIN, XsollaSettings.OAuthClientId, redirectUriParam);

			Action<LoginOAuthJsonResponse> successCallback = response =>
			{
				ProcessOAuthResponse(response);
				onSuccess?.Invoke(response.access_token);
			};

			WebRequestHelper.Instance.PostRequest<LoginOAuthJsonResponse, LoginRequest>(SdkType.Login, url, loginData, successCallback, onError, Error.LoginErrors);
		}

		/// <summary>
		/// Starts authentication by the user email address and sends a confirmation code to their email address.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/api/login/operation/jwt-start-auth-by-email/"/>
		/// <see cref="https://developers.xsolla.com/api/login/operation/oauth-20-start-auth-by-email/"/>
		/// <param name="email">User email address.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the email or not.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void StartAuthByEmail(string email, string linkUrl, bool? sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtStartAuthByEmail(email, linkUrl, sendLink, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthStartAuthByEmail(email, linkUrl, sendLink, onSuccess, onError);
		}

		private void JwtStartAuthByEmail(string email, string linkUrl, bool? sendLink, Action<string> onSuccess, Action<Error> onError = null, string payload = null)
		{
			var data = new StartAuthByEmailRequest(email, linkUrl, sendLink);
			var tokenInvalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			var url = string.Format(URL_JWT_START_AUTH_BY_EMAIL, XsollaSettings.LoginId, XsollaSettings.CallbackUrl, tokenInvalidationFlag, payload);

			WebRequestHelper.Instance.PostRequest<StartAuthByEmailResponse, StartAuthByEmailRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				Error.LoginErrors);
		}

		private void OAuthStartAuthByEmail(string email, string linkUrl, bool? sendLink, Action<string> onSuccess, Action<Error> onError = null, string oauthState = null)
		{
			var data = new StartAuthByEmailRequest(email, linkUrl, sendLink);
			var state = oauthState ?? DEFAULT_OAUTH_STATE;
			var redirectParam = (!string.IsNullOrEmpty(XsollaSettings.CallbackUrl)) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var url = string.Format(URL_OAUTH_START_AUTH_BY_EMAIL, XsollaSettings.OAuthClientId, state, redirectParam);

			WebRequestHelper.Instance.PostRequest<StartAuthByEmailResponse, StartAuthByEmailRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				Error.LoginErrors);
		}

		/// <summary>
		/// Completes authentication by the user email address and a confirmation code.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-complete-auth-by-email/"/>
		/// <param name="email">User email address.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">ID of the confirmation code.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtCompleteAuthByEmail(email, confirmationCode, operationId, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthCompleteAuthByEmail(email, confirmationCode, operationId, onSuccess, onError);
		}

		private void JwtCompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByEmailRequest(email, confirmationCode, operationId);
			var url = string.Format(URL_JWT_COMPLETE_AUTH_BY_EMAIL, XsollaSettings.LoginId);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, CompleteAuthByEmailRequest>(
				SdkType.Login,
				url,
				data,
				response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.token, out var parsedToken))
					{
						Token.Instance = Token.Create(parsedToken);
						onSuccess?.Invoke(Token.Instance);
					}
					else
					{
						onError?.Invoke(Error.UnknownError);
					}
				},
				onError,
				Error.LoginErrors);
		}

		private void OAuthCompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByEmailRequest(email, confirmationCode, operationId);
			var url = string.Format(URL_OAUTH_COMPLETE_AUTH_BY_EMAIL, XsollaSettings.OAuthClientId);

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
				Error.LoginErrors);
		}

		/// <summary>
		/// Starts authentication by the user phone number and sends a confirmation code to their phone number
		/// </summary>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-start-auth-by-phone-number/"/>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the SMS or not.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtStartAuthByPhoneNumber(phoneNumber, linkUrl, sendLink, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthStartAuthByPhoneNumber(phoneNumber, linkUrl, sendLink, onSuccess, onError);
		}

		private void JwtStartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null, string payload = null)
		{
			var data = new StartAuthByPhoneNumberRequest(phoneNumber, linkUrl, sendLink);
			var tokenInvalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			var url = string.Format(URL_JWT_START_AUTH_BY_PHONE_NUMBER, XsollaSettings.LoginId, XsollaSettings.CallbackUrl, tokenInvalidationFlag, payload);

			WebRequestHelper.Instance.PostRequest<StartAuthByPhoneNumberResponse, StartAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				Error.LoginErrors);
		}

		private void OAuthStartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null, string oauthState = null)
		{
			var data = new StartAuthByPhoneNumberRequest(phoneNumber, linkUrl, sendLink);
			var state = oauthState ?? DEFAULT_OAUTH_STATE;
			var redirectParam = (!string.IsNullOrEmpty(XsollaSettings.CallbackUrl)) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var url = string.Format(URL_OAUTH_START_AUTH_BY_PHONE_NUMBER, XsollaSettings.OAuthClientId, state, redirectParam);

			WebRequestHelper.Instance.PostRequest<StartAuthByPhoneNumberResponse, StartAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				data,
				response => onSuccess?.Invoke(response.operation_id),
				onError,
				Error.LoginErrors);
		}

		/// <summary>
		/// Completes authentication by the user phone number and a confirmation code.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-complete-auth-by-phone-number/"/>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">ID of the confirmation code.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtCompleteAuthByPhoneNumber(phoneNumber, confirmationCode, operationId, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthCompleteAuthByPhoneNumber(phoneNumber, confirmationCode, operationId, onSuccess, onError);
		}

		private void JwtCompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByPhoneNumberRequest(phoneNumber, confirmationCode, operationId);
			var url = string.Format(URL_JWT_COMPLETE_AUTH_BY_PHONE_NUMBER, XsollaSettings.LoginId);

			WebRequestHelper.Instance.PostRequest<LoginUrlResponse, CompleteAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				data,
				response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.token, out var parsedToken))
					{
						Token.Instance = Token.Create(parsedToken);
						onSuccess?.Invoke(Token.Instance);
					}
					else
					{
						onError?.Invoke(Error.UnknownError);
					}
				},
				onError,
				Error.LoginErrors);
		}

		private void OAuthCompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			var data = new CompleteAuthByPhoneNumberRequest(phoneNumber, confirmationCode, operationId);
			var url = string.Format(URL_OAUTH_COMPLETE_AUTH_BY_PHONE_NUMBER, XsollaSettings.OAuthClientId);

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
				Error.LoginErrors);
		}

		/// <summary>
		/// Resets the user password with user confirmation.
		/// </summary>
		/// <remarks> Swagger method name:<c>Reset Password</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/reset-password/"/>
		/// <param name="email">Email to send the password change verification message to.</param>
		/// <param name="redirectUri">URL to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the Callback URL specified in the URL block of Publisher Account.
		/// To find it, go to Login > your Login project > General settings. Required if there are several Callback URLs.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="Registration"/>
		/// <seealso cref="SignIn"/>
		public void ResetPassword(string email, string redirectUri = null, Action onSuccess = null, Action<Error> onError = null)
		{
			var proxyParam = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";
			var projectIdParam = XsollaSettings.LoginId;
			var loginUrlParam = (!string.IsNullOrEmpty(redirectUri)) ? redirectUri : XsollaSettings.CallbackUrl;
			var url = string.Format(URL_PASSWORD_RESET, proxyParam, projectIdParam, loginUrlParam);

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, new ResetPassword(email), onSuccess, onError, Error.ResetPasswordErrors);
		}

		/// <summary>
		/// Resends an account confirmation email to a user. To complete account confirmation, the user should follow the link in the email.
		/// </summary>
		/// <remarks> Swagger method name:<c>Resend Confirmation Link</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/jwt/jwt-resend-account-confirmation-email"/>
		/// <see cref="https://developers.xsolla.com/login-api/emails/oauth-20/oauth-20-resend-account-confirmation-email"/>
		/// <param name="username">Username or user email address.</param>
		/// <param name="redirectUri">URL to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the Callback URL specified in the URL block of Publisher Account.
		/// To find it, go to Login > your Login project > General settings. Required if there are several Callback URLs.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 symbols. Used only for OAuth2.0 auth.</param> 
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="Registration"/>
		/// <seealso cref="SignIn"/>
		public void ResendConfirmationLink(string username, string redirectUri = null, string state = null, string payload = null, Action onSuccess = null, Action<Error> onError = null)
		{
			string url;
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var projectIdParam = XsollaSettings.LoginId;
				var loginUrlParam = (!string.IsNullOrEmpty(redirectUri)) ? redirectUri : XsollaSettings.CallbackUrl;
				var payloadParam = (!string.IsNullOrEmpty(payload)) ? $"&payload={payload}" : "";
				url = string.Format(URL_JWT_RESEND_CONFIRMATION_LINK, projectIdParam, loginUrlParam, payloadParam);
			}
			else/*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				var stateParam = state ?? DEFAULT_OAUTH_STATE;
				url = string.Format(URL_OAUTH_RESEND_CONFIRMATION_LINK, XsollaSettings.OAuthClientId, stateParam, DEFAULT_REDIRECT_URI);
			}

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, new ResendConfirmationLinkRequest(username), onSuccess, onError);
		}

		/// <summary>
		/// Authenticates the user with the access token using social network credentials.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Access Token of Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/auth/jwt/jwt-auth-via-access-token-of-social-network/"/>
		/// <see cref="https://developers.xsolla.com/login-api/auth/oauth-20/oauth-20-auth-via-access-token-of-social-network/"/>
		/// <param name="accessToken">Access token received from a social network.</param>
		/// <param name="accessTokenSecret">Parameter oauth_token_secret received from the authorization request. Required for Twitter only.</param>
		/// <param name="openId">Parameter 'openid' received from the social network. Required for WeChat only.</param>
		/// <param name="providerName">Name of the social network connected to the Login in Publisher Account.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		/// <param name="state">Value used for additional user verification. Often used to mitigate CSRF Attacks. The value will be returned in the response. Must be longer than 8 symbols. Used only for OAuth2.0 auth</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string providerName, string payload, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtAuthWithSocialNetworkAccessToken(accessToken, accessTokenSecret, openId, providerName, payload, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthAuthWithSocialNetworkAccessToken(accessToken, accessTokenSecret, openId, providerName, state, onSuccess, onError);
		}

		private void JwtAuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string providerName, string payload, Action<string> onSuccess, Action<Error> onError)
		{
			var tokenInvalidationFlag = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			var url = string.Format(URL_JWT_USER_SOCIAL_NETWORK_TOKEN_AUTH, providerName, XsollaSettings.LoginId, payload, tokenInvalidationFlag);

			var requestData = new SocialNetworkAccessTokenRequest
			{
				access_token = accessToken,
				access_token_secret = accessTokenSecret,
				openId = openId
			};

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, requestData, (TokenEntity result) => { onSuccess?.Invoke(result.token); }, onError, Error.LoginErrors);
		}

		private void OAuthAuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string providerName, string oauthState, Action<string> onSuccess, Action<Error> onError)
		{
			var state = oauthState ?? DEFAULT_OAUTH_STATE;
			var redirectParam = (!string.IsNullOrEmpty(XsollaSettings.CallbackUrl)) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var url = string.Format(URL_OAUTH_USER_SOCIAL_NETWORK_TOKEN_AUTH, providerName, XsollaSettings.OAuthClientId, redirectParam, state);

			var requestData = new SocialNetworkAccessTokenRequest
			{
				access_token = accessToken,
				access_token_secret = accessTokenSecret,
				openId = openId
			};

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, requestData, onSuccess, onError, Error.LoginErrors);
		}

		/// <summary>
		/// Gets user access token provided by game backend.
		/// </summary>
		/// <param name="authParams">Custom authentication parameters.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserAccessToken(AccessTokenAuthParams authParams, Action onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_GET_ACCESS_TOKEN, XsollaSettings.AuthServerUrl);

			WebRequestHelper.Instance.PostRequest<AccessTokenResponse, Dictionary<string, object>>(SdkType.Login, url, authParams.parameters, response =>
			{
				Token.Instance = Token.Create(response.access_token);
				onSuccess?.Invoke();
			}, onError, Error.LoginErrors);
		}

		/// <summary>
		/// Logs the user out and deletes the user session.
		/// </summary>
		/// <remarks> Swagger method name:<c>Log User Out</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/api/login/operation/log-user-out/"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="sessions">Shows how the user is logged out and how the user session is deleted.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void OAuthLogout(string token, OAuthLogoutType sessions, Action onSuccess, Action<Error> onError = null)
		{
			var logoutTypeFlag = sessions.ToString().ToLowerInvariant();
			var url = string.Format(URL_OAUTH_LOGOUT, logoutTypeFlag);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}
