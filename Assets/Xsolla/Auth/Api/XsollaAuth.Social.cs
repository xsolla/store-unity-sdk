using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_JWT_SILENT_AUTH =
			"https://login.xsolla.com/api/social/{0}/cross_auth?projectId={1}{2}{3}&app_id={4}{5}&with_logout={6}&session_ticket={7}{8}&is_redirect=false";

		private const string URL_OAUTH_SILENT_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/cross_auth?client_id={1}&response_type=code&state={2}&redirectUrl={3}&app_id={4}&scope=offline&session_ticket={5}{6}&is_redirect=false";

		private const string URL_JWT_SOCIAL_AUTH =
			"https://login.xsolla.com/api/social/{0}/login_redirect?projectId={1}&with_logout={2}{3}{4}";

		private const string URL_OAUTH_SOCIAL_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/login_redirect?client_id={1}&state={2}&response_type=code&redirectUrl={3}&scope=offline";

		private const string URL_GET_AVAILABLE_SOCIAL_NETWORKS =
			"https://login.xsolla.com/api/users/me/login_urls?{0}";

		/// <summary>
		/// Exchanges the user JWT from Steam for the JWT in your project
		/// Note: this feature doesn't work "out the box" yet.
		/// If you want to enable Steam auth, you need to contact the
		/// support team by email:<see cref="support@xsolla.com"/>.
		/// </summary>
		/// <remarks> Swagger method name:<c>Silent Authentication</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/jwt/jwt-silent-authentication"/>.
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-silent-authentication"/>.
		/// <param name="providerName">Name of the platform the user authorized in. Can be steam, xbox, stone, mailru, abyss.</param>
		/// <param name="appId">Your app ID in the platform.</param>
		/// <param name="sessionTicket">Session ticket received from the platform.</param>
		/// <param name="redirectUrl">URL to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the Callback URL specified in the URL block of Publisher Account. To find it, go to Login > your Login project > General settings. Required if there are several Callback URLs.</param>
		/// <param name="fields">List of parameters which must be requested from the user or social network additionally and written to the JWT.The parameters must be separated by a comma. Used only for JWT auth.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be "xsollatest" by default. Used only for OAuth2.0 auth.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		/// <param name="code">Code received from the platform.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SilentAuth(string providerName, string appId, string sessionTicket, string redirectUrl = null, List<string> fields = null, string oauthState = null, string payload = null, string code = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
				JwtSilentAuth(providerName, appId, sessionTicket, redirectUrl, fields, oauthState, payload, code, onSuccess, onError);
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
				OAuthSilentAuth(providerName, appId, sessionTicket, redirectUrl, fields, oauthState, payload, code, onSuccess, onError);
		}

		private void JwtSilentAuth(string providerName, string appId, string sessionTicket, string redirectUrl = null, List<string> fields = null, string oauthState = null, string payload = null, string code = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var projectIdParam = XsollaSettings.LoginId;
			var loginUrlParam = (!string.IsNullOrEmpty(redirectUrl)) ? $"&login_url={redirectUrl}" : "";
			var fieldsParam = (fields != null && fields.Count > 0) ? $"&fields={string.Join(",", fields)}" : "";
			var payloadParam = (!string.IsNullOrEmpty(payload)) ? $"&payload={payload}" : "";
			var withLogoutParam = XsollaSettings.InvalidateExistingSessions ? "1" : "0";
			var codeParam = (!string.IsNullOrEmpty(code)) ? $"&code={code}" : "";
			var url = string.Format(URL_JWT_SILENT_AUTH, providerName, projectIdParam, loginUrlParam, fieldsParam, appId, payloadParam, withLogoutParam, sessionTicket, codeParam);

			Action<LoginUrlResponse> onSuccessResponse = response =>
			{
				if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.token, out string token))
					onSuccess?.Invoke(token);
				else
					onError?.Invoke(Error.UnknownError);
			};

			WebRequestHelper.Instance.GetRequest<LoginUrlResponse>(SdkType.Login, url, onSuccessResponse, onError);
		}
		
		private void OAuthSilentAuth(string providerName, string appId, string sessionTicket, string redirectUrl = null, List<string> fields = null, string oauthState = null, string payload = null, string code = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var clientIdParam = XsollaSettings.OAuthClientId;
			var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
			var redirectUriParam = (!string.IsNullOrEmpty(redirectUrl)) ? redirectUrl : DEFAULT_REDIRECT_URI;
			var codeParam = (!string.IsNullOrEmpty(code)) ? $"&code={code}" : "";

			var url = string.Format(URL_OAUTH_SILENT_AUTH, providerName, clientIdParam, stateParam, redirectUriParam, appId, sessionTicket, codeParam);

			Action<LoginUrlResponse> onSuccessResponse = response =>
			{
				if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string codeToExchange))
					ExchangeCodeToToken(codeToExchange, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
				else
					onError?.Invoke(Error.UnknownError);
			};
			WebRequestHelper.Instance.GetRequest<LoginUrlResponse>(SdkType.Login, url, onSuccessResponse, onError);
		}

		/// <summary>
		/// Get `url` for social authentication in browser.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-auth-via-social-network"/>.
		/// <param name="providerName">Name of the social network connected to Login in Publisher Account.</param>
		/// <param name="fields">List of parameters which must be requested from the user or social network additionally and written to the JWT. The parameters must be separated by a comma. Used only for JWT auth.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be "xsollatest" by default.</param>
		/// <param name="payload">Your custom data. The value of the parameter will be returned in the payload claim of the user JWT. Used only for JWT auth.</param>
		public string GetSocialNetworkAuthUrl(SocialProvider providerName, string oauthState = null, List<string> fields = null, string payload = null)
		{
			string result = default(string);

			switch (XsollaSettings.AuthorizationType)
			{
				case AuthorizationType.JWT:
					var projectId = XsollaSettings.LoginId;
					var withLogoutParam = XsollaSettings.InvalidateExistingSessions ? "1" : "0";
					var fieldsParam = (fields != null && fields.Count > 0) ? $"&fields={string.Join(",", fields)}" : "";
					var payloadParam = (!string.IsNullOrEmpty(payload)) ? $"&payload={payload}" : "";
					result = string.Format(URL_JWT_SOCIAL_AUTH, providerName.GetParameter(), projectId, withLogoutParam, fieldsParam, payloadParam);
					break;
				case AuthorizationType.OAuth2_0:
					var clientIdParam = XsollaSettings.OAuthClientId;
					var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
					var redirectUriParam = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
					result = string.Format(URL_OAUTH_SOCIAL_AUTH, providerName.GetParameter(), clientIdParam, stateParam, redirectUriParam);
					break;
				default:
					Debug.LogError($"Unexpected authorization type: '{XsollaSettings.AuthorizationType}'");
					return null;
			}

			result = WebRequestHelper.Instance.AppendAnalyticsToUrl(SdkType.Login, result);
			return result;
		}

		/// <summary>
		/// Gets links for authentication via the social networks enabled in your Login project > General settings > Social Networks section of Publisher Account. The links are valid for 10 minutes.
		/// You can get the link by this call and add it to your button for authentication via the social network.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Links for Social Auth</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/getusersmeloginurls/"/>.
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization request localization settings.</param>
		public void GetLinksForSocialAuth(string locale = null, Action<List<SocialNetworkLink>> onSuccess = null, Action<Error> onError = null)
		{
			var localeParam = GetLocaleUrlParam(locale).Replace("&", string.Empty);
			var url = string.Format(URL_GET_AVAILABLE_SOCIAL_NETWORKS, localeParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}
	}
}
