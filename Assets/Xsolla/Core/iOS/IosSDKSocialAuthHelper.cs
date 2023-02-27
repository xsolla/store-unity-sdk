#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Auth;

namespace Xsolla.Core
{
	public class IosSDKSocialAuthHelper
	{
		[DllImport("__Internal")]
		private static extern void _authBySocialNetwork(string platform, int clientID, string state, string redirectUri,
			IosCallbacks.ActionStringCallbackDelegate successCallback, IntPtr successActionPtr,
			IosCallbacks.ActionStringCallbackDelegate errorCallback, IntPtr errorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate cancelCallback, IntPtr cancelActionPtr);

		private Action<LoginOAuthJsonResponse> successCallback;
		private Action cancelCallback;
		private Action<Error> errorCallback;

		public void PerformSocialAuth(SocialProvider socialProvider, Action<LoginOAuthJsonResponse> onSuccess, Action onCancelled, Action<Error> onError)
		{
			successCallback = onSuccess;
			cancelCallback = onCancelled;
			errorCallback = onError;

			var providerName = socialProvider.ToString().ToUpper();
			const string authState = "xsollatest";
			var clientId = XsollaSettings.OAuthClientId;

			var callbackUrl = XsollaSettings.CallbackUrl;
			if (string.IsNullOrEmpty(callbackUrl))
				callbackUrl = $"app://xlogin.{Application.identifier}";

			Action<string> onSuccessNative = SuccessHandler;
			Action<string> onErrorNative = FailHandler;
			Action onCancelNative = CancelHandler;

			_authBySocialNetwork(providerName, clientId, authState, callbackUrl,
				IosCallbacks.ActionStringCallback, onSuccessNative.GetPointer(),
				IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(),
				IosCallbacks.ActionVoidCallback, onCancelNative.GetPointer());
		}

		private void SuccessHandler(string tokenInfo)
		{
			var response = ParseUtils.FromJson<LoginOAuthJsonResponse>(tokenInfo);
			successCallback?.Invoke(response);
		}

		private void FailHandler(string error)
		{
			errorCallback?.Invoke(new Error(errorMessage: $"Social auth failed: {error}"));
		}

		private void CancelHandler()
		{
			cancelCallback?.Invoke();
		}
	}
}
#endif