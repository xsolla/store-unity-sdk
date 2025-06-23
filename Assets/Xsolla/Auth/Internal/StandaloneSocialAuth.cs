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
			var browser = XsollaWebBrowser.InAppBrowser;
			browser.Open(socialNetworkAuthUrl);
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
				if (browser != null)
				{
					browser.CloseEvent += onBrowserClosed;
					browser.UrlChangeEvent += onUrlChanged;
				}
			}

			void unsubscribeEvents()
			{
				if (browser != null)
				{
					browser.CloseEvent -= onBrowserClosed;
					browser.UrlChangeEvent -= onUrlChanged;
				}
			}
		}
	}
}