using System;
using System.Linq;
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

		private const string URL_SOCIAL_AUTH =
            "https://login.xsolla.com/api/social/{0}/login_redirect?projectId={1}&with_logout={2}";

		/// <summary>
		/// Changes Steam session_ticket to JWT.
		/// Note: this feature is not work "out the box" yet.
		/// If you want to enable Steam auth, you need to contact with
		/// support by email:<see cref="support@xsolla.com"/>.
		/// </summary>
		/// <remarks> Swagger method name:<c>Silent Authentication</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/jwt/jwt-silent-authentication"/>.
		/// <param name="appId">Your application Steam AppID.</param>
		/// <param name="sessionTicket">Requested user's session_ticket by SteamAPI.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SteamAuth(string appId, string sessionTicket, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			var projectId = XsollaSettings.LoginId;
			var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			var url = string.Format(URL_STEAM_CROSSAUTH, projectId, appId, with_logout, sessionTicket);
			WebRequestHelper.Instance.GetRequest<CrossAuthResponse>(url, null, response =>
			{
				string[] separator = {"token="};
				var parts = response.login_url.Split(separator, StringSplitOptions.None).ToList();
				if (parts.Count > 1)
				{
					onSuccess?.Invoke(parts.Last());
				}
				else
				{
					onError?.Invoke(Error.UnknownError);
				}
			}, onError);
		}

		/// <summary>
		/// Get `url` for social auth in browser.
		/// </summary>
		/// <remarks> Swagger method name:<c>Auth via Social Network</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
		/// <param name="socialProvider">Name of social provider.</param>
		/// <param name="invalidateTokens">Invalidate other Jwt of this user?</param>
		/// <returns></returns>
		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider)
		{
			var projectId = XsollaSettings.LoginId;
			var with_logout = XsollaSettings.JwtTokenInvalidationEnabled ? "1" : "0";
			return string.Format(URL_SOCIAL_AUTH, socialProvider.GetParameter(), projectId, with_logout);
		}
	}
}