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
			base.OnError?.Invoke(null);
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
				base.OnError?.Invoke(new Error(errorMessage: "Steam auth failed"));
		}
	}

	void RequestTokenBy(string ticket)
	{
		if (int.TryParse(XsollaSettings.SteamAppId, out _))
		{
			DemoController.Instance.GetImplementation().SteamAuth(XsollaSettings.SteamAppId, ticket, SuccessHandler, FailHandler);
		}
		else
		{
			Debug.LogError($"Can't parse SteamAppId = {XsollaSettings.SteamAppId}");
			base.OnError?.Invoke(new Error(errorMessage: "Steam auth failed"));
		}
	}

	void SuccessHandler(string token)
	{
		Debug.Log("SteamAuth.SuccessHandler: Token loaded");
		base.OnSuccess?.Invoke(token);
	}

	void FailHandler(Error error)
	{
		Debug.LogError($"Token request by steam session ticket failed. Ticket: {steamSessionTicket} Error: {error.ToString()}");
		base.OnError?.Invoke(error);
	}
}
