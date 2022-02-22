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

		private const string DEFAULT_REDIRECT_URI = "https://login.xsolla.com/api/blank";

		/// <summary>
		/// Links the social network, which is used by the player for authentication, to the user account.
		/// </summary>
		/// <remarks> Swagger method name:<c>Link Social Network To Account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/link-social-network-to-account/"/>.
		/// <param name="providerName">Name of the social network connected to Login in Publisher Account.</param>
		/// <param name="urlCallback">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void LinkSocialProvider(SocialProvider providerName, Action<string> urlCallback, Action<Error> onError = null)
		{
			var redirectUrl = !string.IsNullOrEmpty(XsollaSettings.CallbackUrl) ? XsollaSettings.CallbackUrl : DEFAULT_REDIRECT_URI;
			var url = string.Format(URL_LINK_SOCIAL_NETWORK, providerName.GetParameter(), redirectUrl);
			WebRequestHelper.Instance.GetRequest<LinkSocialProviderResponse>(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance),
				response => urlCallback?.Invoke(response?.url ?? string.Empty));
		}

		/// <summary>
		/// Gets a list of the social networks linked to the user account.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Linked Networks</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/social-networks/get-linked-networks/"/>.
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, URL_GET_LINKED_SOCIAL_NETWORKS, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}
	}
}
