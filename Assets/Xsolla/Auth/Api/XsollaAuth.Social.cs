using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_SILENT_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/cross_auth?client_id={1}&response_type=code&state={2}&redirect_uri={3}&app_id={4}&scope=offline&session_ticket={5}{6}&is_redirect=false";
		private const string URL_GET_LINK_FOR_SOCIAL_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/login_redirect?client_id={1}&state={2}&response_type=code&redirect_uri={3}&scope=offline";
		private const string URL_GET_AVAILABLE_SOCIAL_NETWORKS =
			"https://login.xsolla.com/api/users/me/login_urls?{0}";

		/// <summary>
		/// Authenticates a user by exchanging the session ticket from Steam, Xbox, or Epic Games to the JWT.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/silent-auth/).</remarks>
		/// <param name="providerName">Platform on which the session ticket was obtained. Can be `steam`, `xbox`, or `epicgames`.</param>
		/// <param name="appId">Platform application identifier.</param>
		/// <param name="sessionTicket">Session ticket received from the platform.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		/// Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		/// Required if there are several URIs.</param>
		/// <param name="fields">[OBSOLETE] List of parameters which must be requested from the user or social network additionally and written to the JWT. The parameters must be separated by a comma. Used only for JWT authorization type.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be `xsollatest` by default. Used only for OAuth2.0 auth.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		/// <param name="code">Code received from the platform.</param>
		/// <param name="onSuccess">Called after successful user authentication with a platform session ticket. Authentication data including a JWT will be received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void SilentAuth(string providerName, string appId, string sessionTicket, string redirectUrl = null, List<string> fields = null, string oauthState = null, string payload = null, string code = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var clientIdParam = XsollaSettings.OAuthClientId;
			var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
			var redirectUriParam = RedirectUtils.GetRedirectUrl(redirectUrl);
			var codeParam = (!string.IsNullOrEmpty(code)) ? $"&code={code}" : "";

			var url = string.Format(URL_SILENT_AUTH, providerName, clientIdParam, stateParam, redirectUriParam, appId, sessionTicket, codeParam);

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
		/// Returns URL for authentication via the specified social network in a browser.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/social-auth/#sdk_how_to_set_up_web_auth_via_social_networks).</remarks>
		/// <param name="providerName">Name of a social network. Provider must be connected to Login in Publisher Account.
		/// Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, or `xbox`.</param>
		/// <param name="fields">[OBSOLETE] List of parameters which must be requested from the user or social network additionally and written to the JWT. The parameters must be separated by a comma. Used only for JWT authorization type.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="payload">[OBSOLETE] Your custom data. Used only for JWT authorization type.</param>
		public string GetSocialNetworkAuthUrl(SocialProvider providerName, string oauthState = null, List<string> fields = null, string payload = null)
		{
			var clientIdParam = XsollaSettings.OAuthClientId;
			var stateParam = (!string.IsNullOrEmpty(oauthState)) ? oauthState : DEFAULT_OAUTH_STATE;
			var redirectUriParam = RedirectUtils.GetRedirectUrl();
			var result = string.Format(URL_GET_LINK_FOR_SOCIAL_AUTH, providerName.GetParameter(), clientIdParam, stateParam, redirectUriParam);

			result = WebRequestHelper.Instance.AppendAnalyticsToUrl(SdkType.Login, result);
			return result;
		}

		/// <summary>
		/// Returns list of links for social authentication enabled in Publisher Account (<b>your Login project > Authentication > Social login</b> section).
		/// The links are valid for 10 minutes.
		/// You can get the link by this call and add it to your button for authentication via the social network.
		/// </summary>
		/// <param name="onSuccess">Called after list of links for social authentication was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale"> Region in the `language code_country code` format, where:
		/// - `language code` — language code in the [ISO 639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) format;
		/// - `country code` — country/region code in the [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2) format.<br/>
		/// The list of the links will be sorted from most to least used social networks, according to the variable value.
		/// </param>
		public void GetLinksForSocialAuth(string locale = null, Action<List<SocialNetworkLink>> onSuccess = null, Action<Error> onError = null)
		{
			var localeParam = GetLocaleUrlParam(locale).Replace("&", string.Empty);
			var url = string.Format(URL_GET_AVAILABLE_SOCIAL_NETWORKS, localeParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetLinksForSocialAuth(locale, onSuccess, onError)));
		}
	}
}
