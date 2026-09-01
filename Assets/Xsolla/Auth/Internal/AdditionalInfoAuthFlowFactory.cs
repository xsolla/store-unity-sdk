using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal static class AdditionalInfoAuthFlowFactory
	{
		public static IAdditionalInfoAuthFlow Create(Action onSuccess, Action<Error> onError, Action onCancel)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return XsollaSettings.InAppBrowserEnabled
				? new StandaloneInAppBrowserAdditionalInfoAuthFlow(onSuccess, onError, onCancel)
				: new StandaloneSystemBrowserAdditionalInfoAuthFlow(onSuccess, onError);
#elif UNITY_ANDROID
			return new AndroidAdditionalInfoAuthFlow(onSuccess, onError, onCancel);
#elif UNITY_WEBGL
			return new WebglAdditionalInfoAuthFlow(onSuccess, onError, onCancel);
#else
			return null;
#endif
		}
	}
}
