using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		/// <summary>
		/// Temporary Steam auth endpoint. Will be changed later.
		/// </summary>
		private const string URL_STEAM_CROSSAUTH =
			"https://login.xsolla.com/api/social/steam/cross_auth?projectId={0}&app_id={1}&with_logout={2}&session_ticket={3}&is_redirect=false";

		private const string URL_OAUT_STEAM_CROSSAUTH =
			"https://login.xsolla.com/api/oauth2/social/steam/cross_auth?app_id={0}&client_id={1}&session_ticket={2}&is_redirect=false&redirect_uri=https://login.xsolla.com/api/blank&response_type=code&scope=offline&state=xsollatest";

		private const string URL_SOCIAL_AUTH =
            "https://login.xsolla.com/api/social/{0}/login_redirect?projectId={1}&with_logout={2}";

		private const string URL_OAUTH_SOCIAL_AUTH =
			"https://login.xsolla.com/api/oauth2/social/{0}/login_redirect?client_id={1}&redirect_uri=https://login.xsolla.com/api/blank&response_type=code&state={2}&scope=offline";

		private const string DEFAULT_OAUTH_STATE = "xsollatest";

		/// <summary>
		/// Changes Steam session_ticket to JWT.
		/// Note: this feature is not work "out the box" yet.
		/// If you want to enable Steam auth, you need to contact with
		/// support by email:<see cref="support@xsolla.com"/>.
		/// </summary>
		/// <remarks> Swagger method name:<c>Silent Authentication</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/jwt/jwt-silent-authentication"/>.
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-silent-authentication"/>.
		/// <param name="appId">Your application Steam AppID.</param>
		/// <param name="sessionTicket">Requested user's session_ticket by SteamAPI.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SteamAuth(string appId, string sessionTicket, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			string url = default(string);
			Action<CrossAuthResponse> onSuccessResponse = null;

			if(XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var projectId = XsollaSettings.LoginId;
				var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
				url = string.Format(URL_STEAM_CROSSAUTH, projectId, appId, with_logout, sessionTicket);

				onSuccessResponse = response =>
				{
					string[] separator = { "token=" };
					var parts = response.login_url.Split(separator, StringSplitOptions.None).ToList();
					if (parts.Count > 1)
					{
						onSuccess?.Invoke(parts.Last());
					}
					else
					{
						onError?.Invoke(Error.UnknownError);
					}
				};
			}
			else/*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				url = string.Format(URL_OAUT_STEAM_CROSSAUTH, appId, XsollaSettings.OAuthClientId, sessionTicket);

				onSuccessResponse = response =>
				{
					if (ParseUtils.TryGetValueFromUrl(response.login_url, ParseParameter.code, out string code))
						XsollaLogin.Instance.ExchangeCodeToToken(code, onSuccessExchange: token => onSuccess?.Invoke(token), onError: onError);
					else
						onError?.Invoke(Error.UnknownError);
				};
			}

			WebRequestHelper.Instance.GetRequest<CrossAuthResponse>(url, null, onSuccessResponse, onError);
		}

		/// <summary>
		/// Get `url` for social auth in browser.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
		/// <param name="socialProvider">Name of social provider.</param>
		/// <param name="oauthState">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be "xsollatest" by default.</param>
		/// <see cref="https://developers.xsolla.com/login-api/methods/oauth-20/oauth-20-auth-via-social-network"/>.
		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider, string oauthState = null)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT)
			{
				var projectId = XsollaSettings.LoginId;
				var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
				return string.Format(URL_SOCIAL_AUTH, socialProvider.GetParameter(), projectId, with_logout);
			}
			else/*if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)*/
			{
				var socialNetwork = socialProvider.GetParameter();
				var clientId = XsollaSettings.OAuthClientId.ToString();
				var state = oauthState ?? DEFAULT_OAUTH_STATE;

				return string.Format(URL_OAUTH_SOCIAL_AUTH, socialNetwork, clientId, state);
			}
		}
	}
}