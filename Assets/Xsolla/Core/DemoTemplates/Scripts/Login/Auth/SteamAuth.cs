using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class SteamAuth : StoreStringActionResult, ILoginAuthorization
{
	private string steamSessionTicket;

	public void TryAuth(params object[] args)
	{
		if (!XsollaSettings.UseSteamAuth)
		{
			Debug.Log("SteamAuth.TryAuth: Steam auth disabled");
			if (base.OnError != null)
				base.OnError.Invoke(null);
		}
		else
		{
			Debug.Log("SteamAuth.TryAuth: Steam auth enabled, trying to get token");

#if UNITY_STANDALONE || UNITY_EDITOR
			steamSessionTicket = new SteamSessionTicket().ToString();
#endif
			if (!string.IsNullOrEmpty(steamSessionTicket))
				RequestTokenBy(steamSessionTicket);
			else
			{
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Steam auth failed"));
			}
		}
	}

	void RequestTokenBy(string ticket)
	{
		int _;
		if (int.TryParse(XsollaSettings.SteamAppId, out _))
		{
			DemoController.Instance.GetImplementation().SteamAuth(XsollaSettings.SteamAppId, ticket, SuccessHandler, FailHandler);
		}
		else
		{
			Debug.LogError(string.Format("Can't parse SteamAppId = {0}", XsollaSettings.SteamAppId));
			if (base.OnError != null)
				base.OnError.Invoke(new Error(errorMessage: "Steam auth failed"));
		}
	}

	void SuccessHandler(string token)
	{
		Debug.Log("SteamAuth.SuccessHandler: Token loaded");
		if (base.OnSuccess != null)
			base.OnSuccess.Invoke(token);
	}

	void FailHandler(Error error)
	{
		Debug.LogError(string.Format("Token request by steam session ticket failed. Ticket: {0} Error: {1}", steamSessionTicket, error.ToString()));
		if (base.OnError != null)
			base.OnError.Invoke(error);
	}
}
