#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using UnityEngine.Device;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public class WebglSocialAuthenticator
	{
		[DllImport("__Internal")]
		private static extern void OpenXsollaSocialAuthPopup(string socialNetworkAuthUrl);

		[DllImport("__Internal")]
		private static extern void OpenXsollaSocialAuthPopupWithConfirmation(string socialNetworkAuthUrl, string popupMessageText, string continueButtonText, string cancelButtonText);

		[DllImport("__Internal")]
		private static extern string GetSocialAuthPopupUrl();

		private readonly SocialProvider SocialProvider;
		private readonly Action SuccessCallback;
		private readonly Action<Error> ErrorCallback;

		public WebglSocialAuthenticator(SocialProvider socialProvider, Action successCallback, Action<Error> errorCallback)
		{
			SocialProvider = socialProvider;
			SuccessCallback = successCallback;
			ErrorCallback = errorCallback;
		}

		public void Launch()
		{
			Screen.fullScreen = false;
			SubscribeToWebCallbacks();

			var socialNetworkAuthUrl = XsollaAuth.GetSocialNetworkAuthUrl(
				SocialProvider,
				GetSocialAuthPopupUrl());

			if (!WebHelper.IsBrowserSafari())
				OpenImmediately(socialNetworkAuthUrl);
			else
				OpenWithConfirmation(socialNetworkAuthUrl);
		}

		private void OpenImmediately(string socialNetworkAuthUrl)
		{
			LogMessage("Open social auth without confirmation");
			OpenXsollaSocialAuthPopup(socialNetworkAuthUrl);
		}

		private void OpenWithConfirmation(string socialNetworkAuthUrl)
		{
			LogMessage("Open social auth with confirmation");
			var browserLocale = WebHelper.GetBrowserLanguage().ToLowerInvariant();

			var localizationProvider = new WidgetOpenConfirmationPopupLocalizationProvider();
			var messageText = localizationProvider.GetMessageText(browserLocale);
			var continueButtonText = localizationProvider.GetContinueButtonText(browserLocale);
			var cancelButtonText = localizationProvider.GetCancelButtonText(browserLocale);
			OpenXsollaSocialAuthPopupWithConfirmation(socialNetworkAuthUrl, messageText, continueButtonText, cancelButtonText);
		}

		private void OnAuthSuccessWebCallbackReceived(string data)
		{
			LogMessage($"OnAuthSuccessWebCallbackReceived. Data: {data}");
			UnsubscribeFromWebCallbacks();

			XsollaAuth.ExchangeCodeToToken(
				data,
				() => SuccessCallback?.Invoke(),
				error => ErrorCallback?.Invoke(error),
				GetSocialAuthPopupUrl());
		}

		private void SubscribeToWebCallbacks()
		{
			XsollaWebCallbacks.Instance.WidgetAuthSuccess += OnAuthSuccessWebCallbackReceived;
		}

		private void UnsubscribeFromWebCallbacks()
		{
			XsollaWebCallbacks.Instance.WidgetAuthSuccess -= OnAuthSuccessWebCallbackReceived;
		}

		private static void LogMessage(string message)
		{
			XDebug.Log($"{nameof(WebglSocialAuthenticator)}: " + message);
		}
	}
}
#endif