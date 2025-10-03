using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	internal class LoginWidgetAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			XsollaAuth.AuthWithXsollaWidget(
				onSuccess,
				error => onError?.Invoke(error),
				() => onError?.Invoke(null));
		}
	}
}