#if UNITY_ANDROID
using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class AndroidAdditionalInfoAuthFlow : IAdditionalInfoAuthFlow
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;

		private bool IsCompleted;

		public AndroidAdditionalInfoAuthFlow(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback)
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

			IsCompleted = false;

			try
			{
				var androidHelper = new AndroidHelper();
				var callback = new AndroidAdditionalInfoAuthCallback(
					androidHelper,
					(code, token) => {
						XDebug.Log($"{nameof(AndroidAdditionalInfoAuthFlow)}: onSuccess callback received");
						HandleSuccess(code, token);
					},
					error => {
						XDebug.LogWarning($"{nameof(AndroidAdditionalInfoAuthFlow)}: onError callback received. Message: {error?.errorMessage}");
						CompleteWithError(error ?? new Error(ErrorType.UnknownError, errorMessage: "Unknown additional info auth error"));
					},
					() => {
						XDebug.Log($"{nameof(AndroidAdditionalInfoAuthFlow)}: onCancel callback received");
						CompleteWithCancel();
					});

				var redirectUrl = RedirectUrlHelper.GetRedirectUrl(null);
				XDebug.Log($"{nameof(AndroidAdditionalInfoAuthFlow)}: launching Android additional info auth flow");

				using (var proxyActivity = new AndroidJavaClass($"{Application.identifier}.androidProxies.AdditionalInfoAuthProxyActivity"))
				{
					proxyActivity.CallStatic("perform", androidHelper.CurrentActivity, callback, loginUrl, redirectUrl);
				}
			}
			catch (Exception exception)
			{
				CompleteWithError(new Error(ErrorType.UnknownError, errorMessage: $"Failed to launch Android additional info auth flow: {exception.Message}"));
			}
		}

		private void HandleSuccess(string code, string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				XsollaToken.Create(token);
				CompleteWithSuccess();
				return;
			}

			if (!string.IsNullOrEmpty(code))
			{
				XsollaAuth.ExchangeCodeToToken(
					code,
					CompleteWithSuccess,
					CompleteWithError);
				return;
			}

			CompleteWithError(new Error(ErrorType.InvalidData, errorMessage: "Additional info auth callback does not contain code or token"));
		}

		private void CompleteWithSuccess()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnSuccessCallback?.Invoke();
		}

		private void CompleteWithError(Error error)
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnErrorCallback?.Invoke(error);
		}

		private void CompleteWithCancel()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnCancelCallback?.Invoke();
		}
	}
}
#endif
