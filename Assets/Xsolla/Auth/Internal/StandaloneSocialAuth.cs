using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneSocialAuth
	{
		public void Perform(SocialProvider provider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			var socialNetworkAuthUrl = XsollaAuth.GetSocialNetworkAuthUrl(provider);
			var browser = XsollaWebBrowser.InAppBrowser;
			browser.Open(socialNetworkAuthUrl);

			browser.AddCloseHandler(() => onCancel?.Invoke());
			browser.AddUrlChangeHandler(url => UrlChangedHandler(url, onSuccess, onError));
		}

		private static void UrlChangedHandler(string url, Action onSuccess, Action<Error> onError)
		{
			if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.code, out var code))
			{
				XsollaAuth.ExchangeCodeToToken(
					code,
					() =>
					{
						XsollaWebBrowser.Close();
						onSuccess?.Invoke();
					},
					onError);
			}
		}
	}
}