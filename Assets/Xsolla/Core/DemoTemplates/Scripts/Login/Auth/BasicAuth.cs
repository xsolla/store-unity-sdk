using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class BasicAuth : StoreStringActionResult, ILoginAuthorization
{
	private const string DEMO_USER_NAME = "XSOLLA";
	private bool _isDemoUser;
	private bool _isJwtInvalidationEnabled;

	public void TryAuth(params object[] args)
	{
		if (TryExtractArgs(args, out string username, out string password, out bool rememberMe))
		{
			_isDemoUser = (username.ToUpper() == DEMO_USER_NAME && password.ToUpper() == DEMO_USER_NAME);
			_isJwtInvalidationEnabled = XsollaSettings.JwtTokenInvalidationEnabled;

			if(_isDemoUser && _isJwtInvalidationEnabled)
				XsollaSettings.JwtTokenInvalidationEnabled = false;

			DemoController.Instance.GetImplementation().SignIn(username, password, rememberMe, BasicAuthSuccess, BasicAuthFailed);
		}
		else
		{
			Debug.LogError("BasicAuth.TryAuth: Could not extract arguments for SignIn");
			base.OnError?.Invoke(new Error(errorMessage: "Basic auth failed"));
		}
	}

	private bool TryExtractArgs(object[] args, out string username, out string password, out bool rememberMe)
	{
		username = default(string);
		password = default(string);
		rememberMe = default(bool);

		if (args == null)
		{
			Debug.LogError("BasicAuth.TryExtractArgs: 'object[] args' was null");
			return false;
		}

		if (args.Length != 3)
		{
			Debug.LogError($"BasicAuth.TryExtractArgs: args.Length expected 3, was {args.Length}");
			return false;
		}

		try
		{
			username = (string)args[0];
			password = (string)args[1];
			rememberMe = (bool)args[2];
		}
		catch (Exception ex)
		{
			Debug.LogError($"BasicAuth.TryExtractArgs: Error during argument extraction: {ex.Message}");
			return false;
		}

		return true;
	}

	private void BasicAuthSuccess()
	{
		RestoreJwtInvalidationIfNeeded();
		base.OnSuccess?.Invoke(DemoController.Instance.GetImplementation().Token);
	}

	private void BasicAuthFailed(Error error)
	{
		RestoreJwtInvalidationIfNeeded();
		Debug.LogWarning($"BasicAuth: auth failed. Error: {error.errorMessage}");
		base.OnError?.Invoke(error);
	}

	private void RestoreJwtInvalidationIfNeeded()
	{
		if(_isDemoUser && _isJwtInvalidationEnabled)
			XsollaSettings.JwtTokenInvalidationEnabled = true;
	}
}
