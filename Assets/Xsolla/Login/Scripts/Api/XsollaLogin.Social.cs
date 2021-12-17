using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		/// <summary>
		/// Temporary Steam auth endpoint. Will be changed later.
		/// </summary>
		private const string URL_JWT_STEAM_CROSSAUTH =
			"https://login.xsolla.com/api/social/steam/cross_auth?projectId={0}&app_id={1}&with_logout={2}&session_ticket={3}&is_redirect=false&payload={4}";

		private const string URL_OAUTH_STEAM_CROSSAUTH =
			"https://login.xsolla.com/api/oauth2/social/steam/cross_auth?app_id={0}&client_id={1}&session_ticket={2}&is_redirect=false&redirect_uri=https://login.xsolla.com/api/blank&response_type=code&scope=offline&state={3}";

		private const string URL_LINK_SOCIAL_NETWORK = 
			"https://login.xsolla.com/api/users/me/social_providers/{0}/login_url?login_url={1}";

		private const string URL_GET_LINKED_SOCIAL_NETWORKS =
			"https://login.xsolla.com/api/users/me/social_providers";

		private const string URL_JWT_SOCIAL_AUTH =
			"https://login.xsolla.com/api/social/{0}/login_redirect?projectId={1}&with_logout={2}&payload={3}&fields={4}";

		private const string URL_OAUTH_SOCIAL_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/login_redirect?client_id={1}&redirect_uri=https://login.xsolla.com/api/blank&response_type=code&state={2}&scope=offline";

		private const string URL_GET_AVAILABLE_SOCIAL_NETWORKS =
			"https://login.xsolla.com/api/users/me/login_urls?{0}";

		private const string DEFAULT_OAUTH_STATE = "xsollatest";

		/// <summary>
		/// Changes Steam session_ticket to JWT.
		/// Note: this feature doesn't work "out the box" yet.
		/// If you want to enable Steam auth, you need to contact the
		/// support team by email:<see cref="support@xsolla.com"/>.
		/// </summary>
		/// <remarks> Swagger method name:<c>Silent Authentication</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/jwt/jwt-silent-authentication"/>.
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-silent-authentication"/>.
		/// <param name="appId">Your application Steam AppID.</param>
		/// <param name="sessionTicket">Requested user's session_ticket by SteamAPI.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be "xsollatest" by default.</param>
		/// <param name="payload">Custom data. The value of the parameter will be returned in the user JWT payload claim.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SteamAuth(string appId, string sessionTicket, string oauthState = null, string payload = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			string url = default(string);
			Action<CrossAuthResponse> onSuccessResponse = null;

			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var projectId = XsollaSettings.LoginId;
				var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
				url = string.Format(URL_JWT_STEAM_CROSSAUTH, projectId, appId, with_logout, sessionTicket, payload);

				onSuccessResponse = response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.token, out string token))
						onSuccess?.Invoke(token);
					else
						onError?.Invoke(Error.UnknownError);
				};
			}
			else /*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				var state = oauthState ?? DEFAULT_OAUTH_STATE;
				url = string.Format(URL_OAUTH_STEAM_CROSSAUTH, appId, XsollaSettings.OAuthClientId, sessionTicket, state);

				onSuccessResponse = response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string code))
						XsollaLogin.Instance.ExchangeCodeToToken(code, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
					else
						onError?.Invoke(Error.UnknownError);
				};
			}

			WebRequestHelper.Instance.GetRequest<CrossAuthResponse>(SdkType.Login, url, onSuccessResponse, onError);
		}

		/// <summary>
		/// Get `url` for social authentication in browser.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
		/// <param name="socialProvider">Name of social provider.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be "xsollatest" by default.</param>
		/// <param name="payload">Custom data. The value of the parameter will be returned in the user JWT > payload claim (JWT only).</param>
		/// <param name="fields">List of parameters which should be requested from the user or social network additionally and written to the token (JWT only).</param>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-auth-via-social-network"/>.
		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider, string oauthState = null, string payload = null, List<string> fields = null)
		{
			string result = default(string);

			switch (XsollaSettings.AuthorizationType)
			{
				case AuthorizationType.JWT:
					var projectId = XsollaSettings.LoginId;
					var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
					var extra_fields = fields != null && fields.Any() ? string.Join(",", fields) : string.Empty;
					result = string.Format(URL_JWT_SOCIAL_AUTH, socialProvider.GetParameter(), projectId, with_logout, payload, extra_fields);
					break;
				case AuthorizationType.OAuth2_0:
					var socialNetwork = socialProvider.GetParameter();
					var clientId = XsollaSettings.OAuthClientId.ToString();
					var state = oauthState ?? DEFAULT_OAUTH_STATE;
					result = string.Format(URL_OAUTH_SOCIAL_AUTH, socialNetwork, clientId, state);
					break;
				default:
					Debug.LogError($"Unexpected authorization type: '{XsollaSettings.AuthorizationType}'");
					return null;
			}

			result = WebRequestHelper.Instance.AppendAnalyticsToUrl(SdkType.Login, result);
			return result;
		}

		/// <summary>
		/// Links the social network, which is used by the player for authentication, to the user account.
		/// </summary>
		/// <remarks> Swagger method name:<c>Link Social Network To Account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/link-social-network-to-account/"/>.
		/// <param name="socialProvider">Name of social provider.</param>
		/// <param name="urlCallback">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void LinkSocialProvider(SocialProvider socialProvider, Action<string> urlCallback, Action<Error> onError = null)
		{
			var redirectUrl = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var url = string.Format(URL_LINK_SOCIAL_NETWORK, socialProvider.GetParameter(), redirectUrl);
			WebRequestHelper.Instance.GetRequest<LinkSocialProviderResponse>(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance),
				response => urlCallback?.Invoke(response?.url ?? string.Empty));
		}

		/// <summary>
		/// Gets a list of the social networks linked to the user account.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Linked Networks</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/get-linked-networks/"/>.
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_GET_LINKED_SOCIAL_NETWORKS, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Gets links for authentication via the social networks enabled in Publisher Account > Login settings > Social Networks.
		/// The links are valid for 10 minutes. You can get the link by this method and add it to your button for authentication via the social network.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Links for Social Auth</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/getusersmeloginurls/"/>.
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization request localization settings.</param>
		public void GetLinksForSocialAuth(Action<List<SocialNetworkLink>> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var localeParam = GetLocaleUrlParam(locale).Replace("&", string.Empty);
			var url = string.Format(URL_GET_AVAILABLE_SOCIAL_NETWORKS, localeParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}
	}
}
