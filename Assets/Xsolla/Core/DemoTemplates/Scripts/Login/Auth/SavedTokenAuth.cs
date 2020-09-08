using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class SavedTokenAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
		if (DemoController.Instance.GetImplementation().LoadToken(Constants.LAST_SUCCESS_AUTH_TOKEN, out var token))
		{
			Debug.Log("SavedTokenAuth.TryAuth: Token loaded");
			base.OnSuccess?.Invoke(token);
		}
		else
		{
			Debug.Log("SavedTokenAuth.TryAuth: No token");
			base.OnError?.Invoke(null);
		}
	}
}
