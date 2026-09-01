#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneInAppBrowserAdditionalInfoAuthFlow : IAdditionalInfoAuthFlow, IInAppBrowserNavigationInterceptor
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly HashSet<string> UrlsToIntercept;

		private bool IsBrowserClosedByCode;
		private bool IsCompleted;

		public StandaloneInAppBrowserAdditionalInfoAuthFlow(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;

			MainThreadExecutor = MainThreadExecutor.Instance;
			UrlsToIntercept = new HashSet<string> {
				"https://login-widget.xsolla.com/latest/ask"
			};
		}

		public void Launch(string loginUrl)
		{
			if (string.IsNullOrEmpty(loginUrl))
			{
				OnAuthError(new Error(ErrorType.InvalidData, errorMessage: "Additional information login URL is null or empty"));
				return;
			}

			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
			{
				OnAuthError(new Error(ErrorType.NotSupportedOnCurrentPlatform, errorMessage: "In-app browser is not available"));
				return;
			}

			var redirectUrl = RedirectUrlHelper.GetRedirectUrl(null);
			UrlsToIntercept.Add(redirectUrl);
			XsollaWebBrowser.Open(loginUrl);
			SubscribeToBrowser();
		}

		private void SubscribeToBrowser()
		{
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent += OnBrowserClosed;
			browser.AddNavigationInterceptor(this);
		}

		private void UnsubscribeFromBrowser()
		{
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent -= OnBrowserClosed;
			browser.RemoveNavigationInterceptor(this);
		}

		private void OnBrowserClosed(BrowserCloseInfo _)
		{
			if (IsBrowserClosedByCode || IsCompleted)
				return;

			IsCompleted = true;
			UnsubscribeFromBrowser();
			OnCancelCallback?.Invoke();
		}

		private void OnAuthSuccess()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			CloseBrowser();
			OnSuccessCallback?.Invoke();
		}

		private void OnAuthError(Error error)
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			CloseBrowser();
			OnErrorCallback?.Invoke(error);
		}

		private void CloseBrowser()
		{
			UnsubscribeFromBrowser();
			IsBrowserClosedByCode = true;
			XsollaWebBrowser.Close();
		}

		public bool ShouldAbortNavigation(string url)
		{
			if (!UrlsToIntercept.Any(x => url.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
				return false;

			if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.token, out var token))
			{
				MainThreadExecutor.Enqueue(() => {
					XsollaToken.Create(token);
					OnAuthSuccess();
				});
				return true;
			}

			if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.code, out var code))
			{
				MainThreadExecutor.Enqueue(() => {
					XsollaAuth.ExchangeCodeToToken(
						code,
						OnAuthSuccess,
						OnAuthError);
				});
				return true;
			}

			return false;
		}
	}
}
#endif
