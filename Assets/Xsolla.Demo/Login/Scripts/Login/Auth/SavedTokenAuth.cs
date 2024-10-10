using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SavedTokenAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			var authViaSavedTokenResult = XsollaAuth.AuthViaSavedToken();
			if (!authViaSavedTokenResult)
			{
				onError?.Invoke(null);
				return;
			}

			if (string.IsNullOrEmpty(XsollaToken.RefreshToken))
			{
				onSuccess?.Invoke();
				return;
			}

			XsollaAuth.RefreshToken(
				() => onSuccess?.Invoke(),
				error => onError?.Invoke(error));
		}
	}
}