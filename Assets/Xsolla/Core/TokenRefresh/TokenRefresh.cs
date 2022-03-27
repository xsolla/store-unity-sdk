using System;

namespace Xsolla.Core
{
	public static class TokenRefresh
    {
		public static Action<Action,Action<Error>> OnInvalidToken;

		public static void HandleError(Error error, Action<Error> onErrorCallback, Action repeatCall)
		{
			if (error.ErrorType == ErrorType.InvalidToken &&
				XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0 &&
				OnInvalidToken != null)
			{
				Debug.Log("TokenRefresh: Attempting to refresh the token");
				OnInvalidToken.Invoke(repeatCall, onErrorCallback);
			}
			else
			{
				onErrorCallback?.Invoke(error);
			}
		}
    }
}
