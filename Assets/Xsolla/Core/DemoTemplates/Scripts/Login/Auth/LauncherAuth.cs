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
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(launcherToken);
		}
		else
		{
			Debug.Log("LauncherAuth.TryAuth: No token");
			if (base.OnError != null)
				base.OnError.Invoke(null);
		}
	}
}
