using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneSocialAuth : IInAppBrowserNavigationInterceptor
	{
		private readonly SocialProvider SocialProvider;
		private readonly Action SuccessCallback;
		private readonly Action<Error> ErrorCallback;
		private readonly Action CancelCallback;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly HashSet<string> UrlsToIntercept;
		private bool IsBrowserClosedByCode;

		public StandaloneSocialAuth(SocialProvider socialProvider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			SocialProvider = socialProvider;
			SuccessCallback = onSuccess;
			ErrorCallback = onError;
			CancelCallback = onCancel;

			MainThreadExecutor = MainThreadExecutor.Instance;
			UrlsToIntercept = new HashSet<string> {
				"https://login-widget.xsolla.com/latest/ask"
			};
		}

		public void Perform()
		{
			var redirectUrl = RedirectUrlHelper.GetRedirectUrl(null);
			UrlsToIntercept.Add(redirectUrl);
			var socialNetworkAuthUrl = XsollaAuth.GetSocialNetworkAuthUrl(SocialProvider, redirectUri: redirectUrl);
			XsollaWebBrowser.Open(socialNetworkAuthUrl);
			SubscribeToBrowser();
		}

		private void SubscribeToBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent += OnBrowserClosed;
			XsollaWebBrowser.InAppBrowser.AddNavigationInterceptor(this);
		}

		private void UnsubscribeFromBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent -= OnBrowserClosed;
			XsollaWebBrowser.InAppBrowser.RemoveNavigationInterceptor(this);
		}

		private void OnBrowserClosed(BrowserCloseInfo info)
		{
			if (IsBrowserClosedByCode)
				return;

			UnsubscribeFromBrowser();
			CancelCallback?.Invoke();
		}

		private void OnAuthSuccess()
		{
			CloseBrowser();
			SuccessCallback?.Invoke();
		}

		private void OnAuthError(Error error)
		{
			CloseBrowser();
			ErrorCallback?.Invoke(error);
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