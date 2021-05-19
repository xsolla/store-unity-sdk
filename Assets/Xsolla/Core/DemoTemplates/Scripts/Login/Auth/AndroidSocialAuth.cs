using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class AndroidSocialAuth : StoreStringActionResult, ILoginAuthorization
{
	private AndroidSDKSocialAuthListener _listener;
	private SocialProvider _requestedProvider;

	public void TryAuth(params object[] args)
	{
		SocialProvider provider;
		if (TryExtractProvider(args, out provider))
		{
			SetListener();
			_requestedProvider = provider;

			try
			{
				using (var sdkHelper = new AndroidSDKSocialAuthHelper())
				{
					sdkHelper.PerformSocialAuth(provider);
				}

				Debug.Log("AndroidSocialAuth.SocialNetworkAuth: auth request was sent");
			}
			catch (Exception ex)
			{
				var message = string.Format("AndroidSocialAuth.SocialNetworkAuth: {0}", ex.Message);
				Debug.LogError(message);
				RemoveListener();

				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
			}
		}
		else
		{
			Debug.LogWarning("AndroidSocialAuth.TryAuth: Could not extract argument");

			if (base.OnError != null)
				base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
		}
	}

	private bool TryExtractProvider(object[] args, out SocialProvider provider)
	{
		provider = default(SocialProvider);

		if (args == null)
		{
			Debug.LogError("AndroidSocialAuth.TryExtractProvider: 'object[] args' was null");
			return false;
		}

		if (args.Length != 1)
		{
			var message = string.Format("AndroidSocialAuth.TryExtractProvider: args.Length expected 1, was {0}", args.Length);
			Debug.LogError(message);
			return false;
		}

		try
		{
			provider = (SocialProvider)args[0];
		}
		catch (Exception ex)
		{
			var message = string.Format("AndroidSocialAuth.TryExtractProvider: Error during argument extraction: {0}", ex.Message);
			Debug.LogError(message);
			return false;
		}

		return true;
	}

	private void SetListener()
	{
		var listenerObject = new GameObject("SocialNetworks");
		listenerObject.transform.parent = this.transform;

		_listener = listenerObject.AddComponent<AndroidSDKSocialAuthListener>();
		_listener.OnSocialAuthResult += OnSocialAuthResult;
	}

	private void RemoveListener()
	{
		_listener.OnSocialAuthResult -= OnSocialAuthResult;
		Destroy(_listener.gameObject);
	}

	private void OnSocialAuthResult(string authResult)
	{
		RemoveListener();

		if (authResult == null)
		{
			Debug.LogError("AndroidSocialAuth.OnSocialAuthResult: authResult was null");
			return;
		}

		var args = authResult.Split('#');

		if (args.Length != 3)
		{
			var argsLengthErrorMessage = string.Format("AndroidSocialAuth.OnSocialAuthResult: args.Length != 3. Result was {0}", authResult);
			Debug.LogError(argsLengthErrorMessage);
			return;
		}

		var socialProvider = args[0];

		if (socialProvider.ToUpper() != _requestedProvider.ToString().ToUpper())
		{
			var wrongProviderErrorMessage = string.Format("AndroidSocialAuth.OnSocialAuthResult: args.Provider was {0} when expected {1}", socialProvider, _requestedProvider);
			Debug.LogError(wrongProviderErrorMessage);
			return;
		}

		var message = string.Format("AndroidSocialAuth.OnSocialAuthResult: processing auth result for {0}", socialProvider);
		Debug.Log(message);

		var authStatus = args[1].ToUpper();
		var messageBody = args[2];

		var logHeader = string.Format("AndroidSocialAuth.OnSocialAuthResult: authResult for {0} returned", socialProvider);

		switch (authStatus)
		{
			case "SUCCESS":
				Debug.Log(string.Format("{0} SUCCESS. Token: {1}", logHeader, messageBody));
				if (base.OnSuccess != null)
					base.OnSuccess.Invoke(messageBody);
				break;
			case "ERROR":
				Debug.LogError(string.Format("{0} ERROR. Error message: {1}", logHeader, messageBody));
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
				break;
			case "CANCELLED":
				Debug.Log(string.Format("{0} CANCELLED. Additional info: {1}", logHeader, messageBody));
				if (base.OnError != null)
					base.OnError.Invoke(null);
				break;
			default:
				Debug.LogError(string.Format("{0} unexpected authResult: {1}", logHeader, authStatus));
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
				break;
		}
	}
}