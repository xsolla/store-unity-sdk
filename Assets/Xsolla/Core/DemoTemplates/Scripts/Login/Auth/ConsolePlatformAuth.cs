using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public class ConsolePlatformAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
		if (XsollaSettings.UseConsoleAuth)
		{
			Debug.Log("ConsolePlatformAuth.TryAuth: Console auth enabled, trying to get token");
			RequestToken();
		}
		else
		{
			Debug.Log("ConsolePlatformAuth.TryAuth: Console auth disabled");

			if (base.OnError != null)
				base.OnError.Invoke(null);
		}
	}

	private void RequestToken()
	{
		DemoController.Instance.GetImplementation().SignInConsoleAccount(
			userId: XsollaSettings.UsernameFromConsolePlatform,
			platform: XsollaSettings.Platform.GetString(),
			successCase: SuccessHandler,
			failedCase: FailHandler);
	}

	private void SuccessHandler(string token)
	{
		Debug.Log("ConsolePlatformAuth.SuccessHandler: Token loaded");
		if (base.OnSuccess != null)
			base.OnSuccess.Invoke(token);
	}

	private void FailHandler(Error error)
	{
		var message = string.Format("Failed request token by console account with user = `{0}` and platform = `{1}`. Error:{2}", XsollaSettings.UsernameFromConsolePlatform, XsollaSettings.Platform.GetString(), error.ToString());
		Debug.LogError(message);
		if (base.OnError != null)
			base.OnError.Invoke(error);
	}
}
