using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class IosSocialAuth : LoginAuthorization
	{
		private const string DEMO_AUTH_STATE = "xsollatest";

#if UNITY_IOS
		[DllImport("__Internal")]
		public static extern void _authBySocialNetwork(string platform, int clientID, string state, string redirectUri,
			IosCallbacks.ActionStringCallbackDelegate onSuccessCallback, IntPtr onSuccessActionPtr,
			IosCallbacks.ActionStringCallbackDelegate onErrorCallback, IntPtr onErrorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate onCancelCallback, IntPtr onCancelActionPtr);
#endif

		private SocialProvider _requestedProvider;

		public override void TryAuth(params object[] args)
		{
			SocialProvider provider;
			if (TryExtractProvider(args, out provider))
			{
				_requestedProvider = provider;

				try
				{

#if UNITY_IOS
					var providerName = _requestedProvider.ToString().ToLower();
					Action<string> onSuccessNative = SuccessHandler;
					Action<string> onErrorNative = FailHandler;
					Action onCancelNative = CancelHandler;
					_authBySocialNetwork(providerName, XsollaSettings.OAuthClientId, DEMO_AUTH_STATE, XsollaSettings.CallbackUrl,
						IosCallbacks.ActionStringCallback, onSuccessNative.GetPointer(),
						IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(),
						IosCallbacks.ActionVoidCallback, onCancelNative.GetPointer());

					Debug.Log("IosSocialAuth.SocialNetworkAuth: auth request was sent");
#else
					Debug.LogError("IosSocialAuth.TryAuth: Platform not supported");
					if (base.OnError != null)
						base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
#endif
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("IosSocialAuth.SocialNetworkAuth: {0}", ex.Message));
					if (base.OnError != null)
						base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
				}
			}
			else
			{
				Debug.LogError("IosSocialAuth.TryAuth: Could not extract argument");
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
			}
		}

		private bool TryExtractProvider(object[] args, out SocialProvider provider)
		{
			provider = default(SocialProvider);

			if (args == null)
			{
				Debug.LogError("IosSocialAuth.TryExtractProvider: 'object[] args' was null");
				return false;
			}

			if (args.Length != 1)
			{
				Debug.LogError(string.Format("IosSocialAuth.TryExtractProvider: args.Length expected 1, was {0}", args.Length));
				return false;
			}

			try
			{
				provider = (SocialProvider)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("IosSocialAuth.TryExtractProvider: Error during argument extraction: {0}", ex.Message));
				return false;
			}

			return true;
		}

		private void SuccessHandler(string tokenInfo)
		{
			Debug.Log("IosSocialAuth.SuccessHandler: Token info received");

			var response = ParseUtils.FromJson<LoginOAuthJsonResponse>(tokenInfo);

			PlayerPrefs.SetString(Constants.LAST_SUCCESS_AUTH_TOKEN, response.access_token);
			PlayerPrefs.SetString(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN, response.refresh_token);

			//Set expiration time for expiration check on next application start
			var expirationTime = DateTime.Now.AddSeconds(response.expires_in);
			PlayerPrefs.SetString(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME, expirationTime.ToString());

			Token.Instance = Token.Create(response.access_token);

			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(response.access_token);
		}

		private void FailHandler(string error)
		{
			Debug.LogError("Social auth failed.");
			if (base.OnError != null)
				base.OnError.Invoke(new Error(errorMessage: string.Format("Social auth failed: {0}", error)));
		}
		
		private void CancelHandler()
		{
			Debug.LogError("Social auth cancelled.");
			if (base.OnError != null)
				base.OnError.Invoke(null);
		}
	}
}
