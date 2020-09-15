using System;
using UnityEngine;
using Xsolla.Core;

public class LauncherAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
		string launcherToken = LauncherArguments.Instance.GetToken();

		if (!string.IsNullOrEmpty(launcherToken))
		{
			Debug.Log("LauncherAuth.TryAuth: Token loaded");
			base.OnSuccess?.Invoke(launcherToken);
		}
		else
		{
			Debug.Log("LauncherAuth.TryAuth: No token");
			base.OnError?.Invoke(null);
		}
	}
}
