using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public class OldConsolePlatformAuth : MonoBehaviour, OldILoginAuthorization
{
	public Action<string> OnSuccess { get;  set; }
	public Action OnFailed { get; set; }
	
	void Start()
	{
		if (XsollaSettings.UseConsoleAuth) {
			RequestToken();
		} else {
			OnFailed();
			Destroy(this, 0.1F);
		}
	}

	private void RequestToken()
	{
		XsollaLogin.Instance.SignInConsoleAccount(
			XsollaSettings.UsernameFromConsolePlatform, 
			XsollaSettings.Platform.GetString(), 
			SuccessHandler, 
			FailedHandler);
	}

	private void SuccessHandler(string token)
	{
		OnSuccess?.Invoke(token);
		Destroy(this, 0.1F);
	}

	private void FailedHandler(Error error)
	{
		Debug.Log(
			"Failed request token by console account with " +
			"user = `" + XsollaSettings.UsernameFromConsolePlatform + 
			"` and platform = `" + XsollaSettings.Platform.GetString() + "`. "
			+ error.ToString());
		OnFailed?.Invoke();
		Destroy(this, 0.1F);
	}
}
