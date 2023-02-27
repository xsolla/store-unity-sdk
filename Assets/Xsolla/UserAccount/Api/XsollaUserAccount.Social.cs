using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_LINK_SOCIAL_NETWORK =
			"https://login.xsolla.com/api/users/me/social_providers/{0}/login_url?login_url={1}";

		private const string URL_GET_LINKED_SOCIAL_NETWORKS =
			"https://login.xsolla.com/api/users/me/social_providers";

		/// <summary>
		/// Links a social network that can be used for authentication to the current account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/account-linking/#sdk_account_linking_additional_account).</remarks>
		/// <param name="providerName">Name of a social network. Provider must be connected to Login in Publisher Account.<br/>
		/// Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `instagram`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paradox`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, `xbox`, `playstation`.</param>
		/// <param name="urlCallback">Called after the URL for social authentication was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void LinkSocialProvider(SocialProvider providerName, Action<string> urlCallback, Action<Error> onError = null)
		{
			var redirectUrl = RedirectUtils.GetRedirectUrl();
			var url = string.Format(URL_LINK_SOCIAL_NETWORK, providerName.GetParameter(), redirectUrl);
			WebRequestHelper.Instance.GetRequest<LinkSocialProviderResponse>(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance),
				onComplete: response => urlCallback?.Invoke(response?.url ?? string.Empty),
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => LinkSocialProvider(providerName, urlCallback, onError)));
		}

		/// <summary>
		/// Returns the list of social networks linked to the user account.
		/// </summary>
		/// <param name="onSuccess">Called after the list of linked social networks was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_GET_LINKED_SOCIAL_NETWORKS, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetLinkedSocialProviders(onSuccess, onError)));
		}
	}
}
