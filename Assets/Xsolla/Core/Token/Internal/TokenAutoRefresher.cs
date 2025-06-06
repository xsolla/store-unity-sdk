using System;
using Xsolla.Auth;

namespace Xsolla.Core
{
	internal static class TokenAutoRefresher
	{
		public static void Check(Error error, Action<Error> onError, Action onSuccess)
		{
			if (error.ErrorType != ErrorType.InvalidToken)
			{
				onError?.Invoke(error);
				return;
			}

			XDebug.Log("Token is invalid. Trying to refresh token");

			XsollaAuth.RefreshToken(
				() => onSuccess?.Invoke(),
				onError);
		}
	}
}