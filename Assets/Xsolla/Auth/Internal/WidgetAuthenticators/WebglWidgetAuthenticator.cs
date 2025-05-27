#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class WebglWidgetAuthenticator : IWidgetAuthenticator
	{
		[DllImport("__Internal")]
		private static extern string OpenXsollaLoginWidgetPopup(string projectId, string locale);

		[DllImport("__Internal")]
		private static extern string OpenXsollaLoginWidgetPopupWithConfirmation(string projectId, string locale, string popupMessageText, string continueButtonText, string cancelButtonText);

		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly string Locale;

		public WebglWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback, string locale)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
			Locale = locale;
		}

		public void Launch()
		{
			Screen.fullScreen = false;
			LogMessage("Launch");
			SubscribeToWebCallbacks();

			if (!WebHelper.IsBrowserSafari())
				OpenImmediately();
			else
				OpenWithConfirmation();
		}

		private void OpenImmediately()
		{
			LogMessage("Open widget without confirmation");
			OpenXsollaLoginWidgetPopup(XsollaSettings.LoginId, Locale);
		}

		private void OpenWithConfirmation()
		{
			LogMessage("Open widget with confirmation");
			var browserLocale = string.IsNullOrEmpty(Locale)
				? WebHelper.GetBrowserLanguage().ToLowerInvariant()
				: Locale.ToLowerInvariant();

			var localizationProvider = new WidgetOpenConfirmationPopupLocalizationDataProvider();
			var messageText = localizationProvider.GetMessageText(browserLocale);
			var continueButtonText = localizationProvider.GetContinueButtonText(browserLocale);
			var cancelButtonText = localizationProvider.GetCancelButtonText(browserLocale);
			OpenXsollaLoginWidgetPopupWithConfirmation(XsollaSettings.LoginId, Locale, messageText, continueButtonText, cancelButtonText);
		}

		private void OnAuthSuccessWebCallbackReceived(string data)
		{
			LogMessage($"OnAuthSuccessWebCallbackReceived. Data: {data}");
			UnsubscribeFromWebCallbacks();
			XsollaToken.Create(data);
			OnSuccessCallback?.Invoke();
		}

		private void OnAuthCancelWebCallbackReceived()
		{
			LogMessage("OnAuthCancelWebCallbackReceived");
			UnsubscribeFromWebCallbacks();
			OnCancelCallback?.Invoke();
		}

		private void SubscribeToWebCallbacks()
		{
			XsollaWebCallbacks.Instance.WidgetAuthSuccess += OnAuthSuccessWebCallbackReceived;
			XsollaWebCallbacks.Instance.WidgetAuthCancel += OnAuthCancelWebCallbackReceived;
		}

		private void UnsubscribeFromWebCallbacks()
		{
			XsollaWebCallbacks.Instance.WidgetAuthSuccess -= OnAuthSuccessWebCallbackReceived;
			XsollaWebCallbacks.Instance.WidgetAuthCancel -= OnAuthCancelWebCallbackReceived;
		}

		private void LogMessage(string message)
		{
			XDebug.Log("WebglWidgetAuthenticator: " + message);
		}
	}
}
#endif