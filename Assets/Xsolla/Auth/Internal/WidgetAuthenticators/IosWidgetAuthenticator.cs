#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class IosWidgetAuthenticator : IWidgetAuthenticator
	{
		[DllImport("__Internal")]
		private static extern void _authWithXsollaWidget(string loginId, int clientId, string state, string redirectUri, string locale,
			IosCallbacks.ActionStringCallbackDelegate successCallback, IntPtr successActionPtr,
			IosCallbacks.ActionStringCallbackDelegate errorCallback, IntPtr errorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate cancelCallback, IntPtr cancelActionPtr);

		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly string Locale;

		public IosWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback, string locale)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
			Locale = locale;
		}

		public void Launch()
		{
			IosUtils.ConfigureAnalytics();

			Action<string> onSuccessNative = HandleSuccess;
			Action<string> onErrorNative = HandleError;
			Action onCancelNative = HandleCancel;

			_authWithXsollaWidget(
				XsollaSettings.LoginId,
				XsollaSettings.OAuthClientId,
				"xsollatest",
				RedirectUrlHelper.GetAuthDeepLinkUrl(),
				Locale,
				IosCallbacks.ActionStringCallback, onSuccessNative.GetPointer(),
				IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(),
				IosCallbacks.ActionVoidCallback, onCancelNative.GetPointer());
		}

		private void HandleSuccess(string tokenJson)
		{
			var response = ParseUtils.FromJson<TokenResponse>(tokenJson);
			XsollaToken.Create(response.access_token, response.refresh_token);
			OnSuccessCallback?.Invoke();
		}

		private void HandleError(string error)
		{
			OnErrorCallback?.Invoke(new Error(errorMessage: $"IosSocialAuth: {error}"));
		}

		private void HandleCancel()
		{
			OnCancelCallback?.Invoke();
		}
	}
}
#endif