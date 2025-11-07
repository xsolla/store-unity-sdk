#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla
{
	internal class StandaloneSystemBrowserWidgetAuthenticator : IWidgetAuthenticator
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;

		public StandaloneSystemBrowserWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
		}

		public void Launch()
		{
			var url = new UrlBuilder("https://login-widget.xsolla.com/latest/")
				.AddProjectId(XsollaSettings.LoginId)
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType("code")
				.AddState("xsollatest")
				.AddRedirectUri(XsollaSettings.LocalServerRedirectUrl)
				.AddScope("offline")
				.Build();

			Application.OpenURL(url);
			LocalAuthServer.Start(XsollaSettings.LocalServerRedirectUrl, OnSuccessCallback, OnErrorCallback);
		}
	}
}
#endif