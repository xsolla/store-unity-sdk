#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneSystemBrowserAdditionalInfoAuthFlow : IAdditionalInfoAuthFlow
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;

		public StandaloneSystemBrowserAdditionalInfoAuthFlow(Action onSuccessCallback, Action<Error> onErrorCallback)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
		}

		public void Launch(string loginUrl)
		{
			if (string.IsNullOrEmpty(loginUrl))
			{
				OnErrorCallback?.Invoke(new Error(ErrorType.InvalidData, errorMessage: "Additional information login URL is null or empty"));
				return;
			}

			Application.OpenURL(loginUrl);
			LocalAuthServer.Start(XsollaSettings.LocalServerRedirectUrl, OnSuccessCallback, OnErrorCallback, null, SdkType.Login);
		}
	}
}
#endif
