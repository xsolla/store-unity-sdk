#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class WebglAdditionalInfoAuthFlow : IAdditionalInfoAuthFlow
	{
		[Serializable]
		private class AdditionalInfoAuthPayload
		{
			public string code;
			public string token;
			public string error;
		}

		[DllImport("__Internal")]
		private static extern void OpenXsollaAdditionalInfoAuthPopup(string loginUrl);

		[DllImport("__Internal")]
		private static extern void OpenXsollaAdditionalInfoAuthPopupWithConfirmation(string loginUrl, string popupMessageText, string continueButtonText, string cancelButtonText);

		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;

		private bool IsCompleted;

		public WebglAdditionalInfoAuthFlow(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
		}

		public void Launch(string loginUrl)
		{
			if (string.IsNullOrEmpty(loginUrl))
			{
				OnErrorCallback?.Invoke(new Error(ErrorType.InvalidData, errorMessage: "Additional information login URL is null or empty"));
				return;
			}

			UnsubscribeFromWebCallbacks();
			IsCompleted = false;

			Screen.fullScreen = false;
			SubscribeToWebCallbacks();

			if (!WebHelper.IsBrowserSafari())
				OpenXsollaAdditionalInfoAuthPopup(loginUrl);
			else
				OpenWithConfirmation(loginUrl);
		}

		private void OpenWithConfirmation(string loginUrl)
		{
			var browserLocale = WebHelper.GetBrowserLanguage().ToLowerInvariant();
			var localizationProvider = new WidgetOpenConfirmationPopupLocalizationProvider();
			var messageText = localizationProvider.GetMessageText(browserLocale);
			var continueButtonText = localizationProvider.GetContinueButtonText(browserLocale);
			var cancelButtonText = localizationProvider.GetCancelButtonText(browserLocale);
			OpenXsollaAdditionalInfoAuthPopupWithConfirmation(loginUrl, messageText, continueButtonText, cancelButtonText);
		}

		private void OnAdditionalInfoAuthSuccessReceived(string payloadJson)
		{
			if (IsCompleted)
				return;

			var payload = ParseUtils.FromJson<AdditionalInfoAuthPayload>(payloadJson);
			if (payload == null)
			{
				CompleteWithError(new Error(ErrorType.InvalidData, errorMessage: "Additional info auth payload is invalid"));
				return;
			}

			if (!string.IsNullOrEmpty(payload.error))
			{
				CompleteWithError(new Error(ErrorType.UnknownError, errorMessage: payload.error));
				return;
			}

			if (!string.IsNullOrEmpty(payload.token))
			{
				XsollaToken.Create(payload.token);
				CompleteWithSuccess();
				return;
			}

			if (!string.IsNullOrEmpty(payload.code))
			{
				XsollaAuth.ExchangeCodeToToken(
					payload.code,
					CompleteWithSuccess,
					CompleteWithError);
				return;
			}

			CompleteWithError(new Error(ErrorType.InvalidData, errorMessage: "Additional info auth payload has no token or code"));
		}

		private void OnAdditionalInfoAuthCancelReceived(string reason)
		{
			if (IsCompleted)
				return;

			UnsubscribeFromWebCallbacks();
			IsCompleted = true;
			OnCancelCallback?.Invoke();
		}

		private void CompleteWithSuccess()
		{
			if (IsCompleted)
				return;

			UnsubscribeFromWebCallbacks();
			IsCompleted = true;
			OnSuccessCallback?.Invoke();
		}

		private void CompleteWithError(Error error)
		{
			if (IsCompleted)
				return;

			UnsubscribeFromWebCallbacks();
			IsCompleted = true;
			OnErrorCallback?.Invoke(error);
		}

		private void SubscribeToWebCallbacks()
		{
			XsollaWebCallbacks.Instance.AdditionalInfoAuthSuccess += OnAdditionalInfoAuthSuccessReceived;
			XsollaWebCallbacks.Instance.AdditionalInfoAuthCancel += OnAdditionalInfoAuthCancelReceived;
		}

		private void UnsubscribeFromWebCallbacks()
		{
			XsollaWebCallbacks.Instance.AdditionalInfoAuthSuccess -= OnAdditionalInfoAuthSuccessReceived;
			XsollaWebCallbacks.Instance.AdditionalInfoAuthCancel -= OnAdditionalInfoAuthCancelReceived;
		}
	}
}
#endif
