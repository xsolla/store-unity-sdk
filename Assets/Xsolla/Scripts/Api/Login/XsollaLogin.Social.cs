using System;
using UnityEngine;
using Xsolla.Core;
using System.Net;
using System.Net.Sockets;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		/// <summary>
		/// Temporary Steam auth endpoint. Will be changed later.
		/// </summary>
		private const string URL_STEAM_CROSSAUTH = "https://livedemo.xsolla.com/sdk/token/steam";

		private const string URL_SOCIAL_AUTH =
			"https://login.xsolla.com/api/social/{1}/login_redirect?projectId={0}";

		/// <summary>
		/// Changes Steam session_ticket to JWT.
		/// Note: this feature is not work "out the box" yet.
		/// If you want to enable Steam auth, you need to contact with
		/// support by email:<see cref="support@xsolla.com".
		/// </summary>
		/// <remarks> Swagger method name:<c>Cross-auth</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/cross-auth/"/>.
		/// <param name="appId">Your application Steam AppID.</param>
		/// <param name="sessionTicket">Requested user's session_ticket by SteamAPI.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SteamAuth(string appId, string sessionTicket, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			string url = URL_STEAM_CROSSAUTH;
			url += "?projectId=" + XsollaSettings.LoginId;
			url += "&app_id=" + appId;
			url += "&session_ticket=" + sessionTicket;
			WebRequestHelper.Instance.GetRequest<TokenEntity>(url, null, token => onSuccess?.Invoke(token.token), onError);
		}

		/// <summary>
		/// Get `url` for social auth in browser.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
		/// <param name="socialProvider">Name of social provider.</param>
		/// <param name="invalidateTokens">Invalidate other Jwt of this user?</param>
		/// <returns></returns>
		public string GetSocialNetworkAuthUrl(
			SocialProvider socialProvider,
			bool invalidateTokens = false)
		{
			var url = string.Format(URL_SOCIAL_AUTH, XsollaSettings.LoginId, socialProvider.GetParameter());
			return invalidateTokens ? (url + "&with_logout=1") : url;
		}
	}
}