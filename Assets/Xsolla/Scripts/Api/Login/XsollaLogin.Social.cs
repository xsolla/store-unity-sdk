using System;
using UnityEngine;
using System.Text;
using Xsolla.Core;
using System.Collections.Generic;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		/// <summary>
		/// Temporary Steam auth endpoint. Will be changed later.
		/// </summary>
		private const string URL_STEAM_CROSSAUTH = "https://livedemo.xsolla.com/sdk/token/steam";

		/// <summary>
		/// Changes Steam session_ticket to JWT.
		/// </summary>
		/// <remarks>Documentation will be later.</remarks>
		/// <param name="appId">Your application Steam AppID</param>
		/// <param name="sessionTicket">Requested user's session_ticket by SteamAPI</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void SteamAuth(string appId, string sessionTicket, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			string url = URL_STEAM_CROSSAUTH;
			url += "?projectId=" + XsollaSettings.LoginId;
			url += "&app_id=" + appId;
			url += "&session_ticket=" + sessionTicket;
			WebRequestHelper.Instance.GetRequest<TokenEntity>(url, null, (TokenEntity token) => onSuccess?.Invoke(token.token), onError);
		}
	}
}