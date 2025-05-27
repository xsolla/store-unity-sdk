using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class ByWidgetAuthenticator : IAuthenticator
	{
		private readonly string Locale;

		public ByWidgetAuthenticator(string locale)
		{
			Locale = locale;
		}

		public void Execute(Action onSuccess, Action<Error> onError, Action onCancel, Action onSkip)
		{
			XsollaAuth.AuthWithXsollaWidget(
				() => onSuccess?.Invoke(),
				e => onError?.Invoke(e),
				() => onCancel?.Invoke(),
				Locale,
				SdkType.ReadyToUseStore);
		}
	}
}