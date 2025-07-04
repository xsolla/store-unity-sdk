using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneSocialAuth
	{
		private bool isBrowserClosedByCode;

		public void Perform(SocialProvider provider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			var socialNetworkAuthUrl = XsollaAuth.GetSocialNetworkAuthUrl(provider);
			XsollaWebBrowser.Open(socialNetworkAuthUrl);
			subscribeEvents();
			return;

			void onBrowserClosed(BrowserCloseInfo info)
			{
				if (!isBrowserClosedByCode)
				{
					unsubscribeEvents();
					onCancel?.Invoke();
				}
			}

			void onUrlChanged(string url)
			{
				if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.code, out var code))
				{
					XsollaAuth.ExchangeCodeToToken(
						code,
						() => {
							isBrowserClosedByCode = true;
							unsubscribeEvents();
							XsollaWebBrowser.Close();
							onSuccess?.Invoke();
						},
						onError);
				}
			}

			void subscribeEvents()
			{
				var browser = XsollaWebBrowser.InAppBrowser;
				if (browser != null)
				{
					browser.CloseEvent += onBrowserClosed;
					browser.UrlChangeEvent += onUrlChanged;
				}
			}

			void unsubscribeEvents()
			{
				var browser = XsollaWebBrowser.InAppBrowser;
				if (browser != null)
				{
					browser.CloseEvent -= onBrowserClosed;
					browser.UrlChangeEvent -= onUrlChanged;
				}
			}
		}
	}
}