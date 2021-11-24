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
					Debug.LogError(string.Format("AndroidSocialAuth.SocialNetworkAuth: {0}", ex.Message));
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
				Debug.LogError(string.Format("AndroidSocialAuth.TryExtractProvider: args.Length expected 1, was {0}", args.Length));
				return false;
			}

			try
			{
				provider = (SocialProvider)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("AndroidSocialAuth.TryExtractProvider: Error during argument extraction: {0}", ex.Message));
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
				Debug.LogError(string.Format("AndroidSocialAuth.OnSocialAuthResult: args.Provider was {0} when expected {1}", socialProvider, _requestedProvider));
				return;
			}

			Debug.Log(string.Format("AndroidSocialAuth.OnSocialAuthResult: processing auth result for {0}", socialProvider));

			var authStatus = args[1].ToUpper();
			var messageBody = args.Length == 3 ? args[2] : string.Empty;
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
}
