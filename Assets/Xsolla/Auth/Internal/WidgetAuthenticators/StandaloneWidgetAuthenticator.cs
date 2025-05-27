#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneWidgetAuthenticator : IWidgetAuthenticator
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly string Locale;
		private readonly SdkType SdkType;

		public StandaloneWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback, string locale, SdkType sdkType)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
			Locale = locale;
			SdkType = sdkType;
		}

		public void Launch()
		{
			var url = new UrlBuilder("https://login-widget.xsolla.com/latest/")
				.AddProjectId(XsollaSettings.LoginId)
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType("code")
				.AddState("xsollatest")
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(null))
				.AddScope("offline")
				.AddLocale(Locale)
				.Build();

			XsollaWebBrowser.Open(url);
			SubscribeToBrowser();
			XsollaWebBrowser.InAppBrowser.UpdateSize(820, 840);
		}

		private void OnBrowserUrlChange(string newUrl)
		{
			if (!ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out var parsedCode))
				return;

			UnsubscribeFromBrowser();
			XsollaWebBrowser.Close();

			XsollaAuth.ExchangeCodeToToken(
				parsedCode,
				() => OnSuccessCallback?.Invoke(),
				error => OnErrorCallback?.Invoke(error),
				sdkType: SdkType);
		}

		private void OnBrowserClose(BrowserCloseInfo info)
		{
			OnCancelCallback?.Invoke();
			UnsubscribeFromBrowser();
		}

		private void SubscribeToBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			browser.CloseEvent += OnBrowserClose;
			browser.UrlChangeEvent += OnBrowserUrlChange;
		}

		private void UnsubscribeFromBrowser()
		{
			// TODO Case when browser is null (whole XsollaWebBrowser folder was removed)
			var browser = XsollaWebBrowser.InAppBrowser;
			browser.CloseEvent -= OnBrowserClose;
			browser.UrlChangeEvent -= OnBrowserUrlChange;
		}
	}
}
#endif