using System;
using System.Collections.Generic;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public void SteamAuth(string appId, string sessionTicket, string redirect_uri = null, List<string> fields = null, string oauthState = null, string payload = null, string code = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.SilentAuth("steam", appId, sessionTicket, redirect_uri, fields, oauthState, payload, code, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public string GetSocialNetworkAuthUrl(SocialProvider providerName, string oauthState = null, List<string> fields = null, string payload = null)
			=> XsollaAuth.Instance.GetSocialNetworkAuthUrl(providerName, oauthState, fields, payload);

		[Obsolete("Use XsollaUserAccount instead")]
		public void LinkSocialProvider(SocialProvider providerName, Action<string> urlCallback, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.LinkSocialProvider(providerName, urlCallback, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.GetLinkedSocialProviders(onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void GetLinksForSocialAuth(string locale = null, Action<List<SocialNetworkLink>> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.GetLinksForSocialAuth(locale, onSuccess, onError);
	}
}
