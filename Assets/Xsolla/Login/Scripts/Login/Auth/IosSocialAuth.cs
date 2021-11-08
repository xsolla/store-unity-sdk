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
			IosCallbacks.ActionVoidCallbackDelegate onErrorCallback, IntPtr onErrorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate onCancelCallback, IntPtr onCancelActionPtr);
#endif

		private SocialProvider _requestedProvider;

		public override void TryAuth(params object[] args)
		{
			if (TryExtractProvider(args, out SocialProvider provider))
			{
				_requestedProvider = provider;

				try
				{
					var providerName = _requestedProvider.ToString().ToLower();

					Action<string> onSuccessNative = SuccessHandler;
					Action onErrorNative = FailHandler;
					Action onCancelNative = CancelHandler;
#if UNITY_IOS
					_authBySocialNetwork(providerName, XsollaSettings.OAuthClientId, DEMO_AUTH_STATE, XsollaSettings.CallbackUrl,
						IosCallbacks.ActionStringCallback, onSuccessNative.GetPointer(),
						IosCallbacks.ActionVoidCallback, onErrorNative.GetPointer(),
						IosCallbacks.ActionVoidCallback, onCancelNative.GetPointer());

					Debug.Log("IosSocialAuth.SocialNetworkAuth: auth request was sent");
#else
					Debug.LogError("IosSocialAuth.TryAuth: Platform not supported");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
#endif
				}
				catch (Exception ex)
				{
					Debug.LogError($"IosSocialAuth.SocialNetworkAuth: {ex.Message}");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
				}
			}
			else
			{
				Debug.LogError("IosSocialAuth.TryAuth: Could not extract argument");
				base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
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
				Debug.LogError($"IosSocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				provider = (SocialProvider)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError($"IosSocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
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

			base.OnSuccess?.Invoke(response.access_token);
		}

		private void FailHandler()
		{
			Debug.LogError($"Social auth failed.");
			base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
		}
		
		private void CancelHandler()
		{
			Debug.LogError($"Social auth cancelled.");
			base.OnError?.Invoke(null);
		}
	}
}