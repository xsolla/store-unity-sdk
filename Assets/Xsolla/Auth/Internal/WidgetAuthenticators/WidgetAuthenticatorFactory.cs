using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class WidgetAuthenticatorFactory
	{
		public IWidgetAuthenticator Create(Action onSuccess, Action<Error> onError, Action onCancel, string locale, SdkType sdkType)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return new StandaloneWidgetAuthenticator(onSuccess, onError, onCancel, locale, sdkType);
#elif UNITY_ANDROID
			return new AndroidWidgetAuthenticator(onSuccess, onError, onCancel, locale);
#elif UNITY_IOS
			return new IosWidgetAuthenticator(onSuccess, onError, onCancel, locale);
#elif UNITY_WEBGL
			return new WebglWidgetAuthenticator(onSuccess, onError, onCancel, locale);
#else
			return null;
#endif
		}
	}
}