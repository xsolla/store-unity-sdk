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
		string username; string password; bool rememberMe;
		if (TryExtractArgs(args, out username, out password, out rememberMe))
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
			if (base.OnError != null)
				base.OnError.Invoke(new Error(errorMessage: "Basic auth failed"));
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
			var message = string.Format("BasicAuth.TryExtractArgs: args.Length expected 3, was {0}", args.Length);
			Debug.LogError(message);
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
			var message = string.Format("BasicAuth.TryExtractArgs: Error during argument extraction: {0}", ex.Message);
			Debug.LogError(message);
			return false;
		}

		return true;
	}

	private void BasicAuthSuccess()
	{
		RestoreJwtInvalidationIfNeeded();
		if (base.OnSuccess != null)
			base.OnSuccess.Invoke(DemoController.Instance.GetImplementation().Token);
	}

	private void BasicAuthFailed(Error error)
	{
		RestoreJwtInvalidationIfNeeded();
		var message = string.Format("BasicAuth: auth failed. Error: {0}", error.errorMessage);
		Debug.LogWarning(message);
		if (base.OnError != null)
			base.OnError.Invoke(error);
	}

	private void RestoreJwtInvalidationIfNeeded()
	{
		if(_isDemoUser && _isJwtInvalidationEnabled)
			XsollaSettings.JwtTokenInvalidationEnabled = true;
	}
}
