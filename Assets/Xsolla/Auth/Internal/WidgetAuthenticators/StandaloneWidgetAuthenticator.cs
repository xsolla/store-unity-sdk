#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneWidgetAuthenticator : IWidgetAuthenticator, IInAppBrowserNavigationInterceptor
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly string Locale;
		private readonly SdkType SdkType;
		private readonly MainThreadExecutor MainThreadExecutor;
		private readonly HashSet<string> UrlsToIntercept;
		private bool IsBrowserClosedByCode;

		public StandaloneWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback, string locale, SdkType sdkType)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
			Locale = locale;
			SdkType = sdkType;

			MainThreadExecutor = MainThreadExecutor.Instance;
			UrlsToIntercept = new HashSet<string> {
				"https://login-widget.xsolla.com/latest/ask"
			};
		}

		public void Launch()
		{
			var redirectUrl = RedirectUrlHelper.GetRedirectUrl(null);
			UrlsToIntercept.Add(redirectUrl);

			var url = new UrlBuilder("https://login-widget.xsolla.com/latest/")
				.AddProjectId(XsollaSettings.LoginId)
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType("code")
				.AddState("xsollatest")
				.AddRedirectUri(redirectUrl)
				.AddScope("offline")
				.AddLocale(Locale)
				.Build();

			XsollaWebBrowser.Open(url);
			SubscribeToBrowser();
			XsollaWebBrowser.InAppBrowser.UpdateSize(820, 840);
		}

		private void OnAuthSuccess()
		{
			CloseBrowser();
			OnSuccessCallback?.Invoke();
		}

		private void OnAuthError(Error error)
		{
			CloseBrowser();
			OnErrorCallback?.Invoke(error);
		}

		private void CloseBrowser()
		{
			UnsubscribeFromBrowser();
			IsBrowserClosedByCode = true;
			XsollaWebBrowser.Close();
		}

		private void OnBrowserClose(BrowserCloseInfo info)
		{
			if (IsBrowserClosedByCode)
				return;

			UnsubscribeFromBrowser();
			OnCancelCallback?.Invoke();
		}

		private void SubscribeToBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent += OnBrowserClose;
			XsollaWebBrowser.InAppBrowser.AddNavigationInterceptor(this);
		}

		private void UnsubscribeFromBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			if (browser == null)
				return;

			browser.CloseEvent -= OnBrowserClose;
			XsollaWebBrowser.InAppBrowser.RemoveNavigationInterceptor(this);
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
						OnAuthError,
						sdkType: SdkType);
				});
				return true;
			}

			return false;
		}
	}
}
#endif