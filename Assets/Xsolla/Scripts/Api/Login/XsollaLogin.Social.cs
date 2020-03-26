using System;
using UnityEngine;
using System.Text;
using Xsolla.Core;
using System.Collections.Generic;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		//private const string URL_STEAM_CROSSAUTH = "https://login.xsolla.com/api/social/steam/cross_auth";
		private const string URL_STEAM_CROSSAUTH = "https://livedemo.xsolla.com/sdk/token/steam";

		public void SteamAuth(string appId, string sessionTicket, Action<string> success = null, Action<Error> failed = null)
		{
			string url = URL_STEAM_CROSSAUTH;
			url += "?projectId=" + XsollaSettings.LoginId;
			url += "&app_id=" + appId;
			url += "&session_ticket=" + sessionTicket;
			WebRequestHelper.Instance.GetRequest<Token>(url, null, (Token token) => success?.Invoke(token.token), failed);
		}
	}
}