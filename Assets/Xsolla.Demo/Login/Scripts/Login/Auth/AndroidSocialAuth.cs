using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class AndroidSocialAuth : LoginAuthorization
	{
		private AndroidSDKSocialAuthListener _listener;
		private SocialProvider _requestedProvider;

		public override void TryAuth(params object[] args)
		{
			if (TryExtractProvider(args, out SocialProvider provider))
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
					Debug.LogError($"AndroidSocialAuth.SocialNetworkAuth: {ex.Message}");
					RemoveListener();
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
				}
			}
			else
			{
				Debug.LogWarning("AndroidSocialAuth.TryAuth: Could not extract argument");
				base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
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
				Debug.LogError($"AndroidSocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				provider = (SocialProvider)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError($"AndroidSocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
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

			var socialProvider = args[0];

			if (socialProvider.ToUpper() != _requestedProvider.ToString().ToUpper())
			{
				Debug.LogError($"AndroidSocialAuth.OnSocialAuthResult: args.Provider was {socialProvider} when expected {_requestedProvider}");
				return;
			}

			Debug.Log($"AndroidSocialAuth.OnSocialAuthResult: processing auth result for {socialProvider}");

			var authStatus = args[1].ToUpper();
			var messageBody = args.Length == 3 ? args[2] : string.Empty;
			var logHeader = $"AndroidSocialAuth.OnSocialAuthResult: authResult for {socialProvider} returned";

			switch (authStatus)
			{
				case "SUCCESS":
					Debug.Log($"{logHeader} SUCCESS. Token: {messageBody}");
					base.OnSuccess?.Invoke(messageBody);
					break;
				case "ERROR":
					Debug.LogError($"{logHeader} ERROR. Error message: {messageBody}");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
					break;
				case "CANCELLED":
					Debug.Log($"{logHeader} CANCELLED. Additional info: {messageBody}");
					base.OnError?.Invoke(null);
					break;
				default:
					Debug.LogError($"{logHeader} unexpected authResult: {authStatus}");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
					break;
			}
		}
	}
}
