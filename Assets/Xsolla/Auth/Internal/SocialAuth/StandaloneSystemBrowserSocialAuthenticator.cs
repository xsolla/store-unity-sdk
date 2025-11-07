#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneSystemBrowserSocialAuthenticator
	{
		private readonly SocialProvider SocialProvider;
		private readonly Action SuccessCallback;
		private readonly Action<Error> ErrorCallback;

		public StandaloneSystemBrowserSocialAuthenticator(SocialProvider socialProvider, Action successCallback, Action<Error> errorCallback, Action cancelCallback)
		{
			SocialProvider = socialProvider;
			SuccessCallback = successCallback;
			ErrorCallback = errorCallback;
		}

		public void Perform()
		{
			var socialNetworkAuthUrl = XsollaAuth.GetSocialNetworkAuthUrl(
				SocialProvider,
				redirectUri: XsollaSettings.LocalServerRedirectUrl);

			Application.OpenURL(socialNetworkAuthUrl);
			LocalAuthServer.Start(XsollaSettings.LocalServerRedirectUrl, SuccessCallback, ErrorCallback);
		}
	}
}
#endif