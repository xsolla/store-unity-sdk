using System;
using UnityEngine;

namespace Xsolla.Core
{
	public class TokenRefresh : MonoSingleton<TokenRefresh>
    {
		private string _refreshToken;

		public string RefreshToken
		{
			get
			{
				if (string.IsNullOrEmpty(_refreshToken))
					_refreshToken = PlayerPrefs.GetString(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN, string.Empty);

				return _refreshToken;
			}
			set
			{
				_refreshToken = value;
				PlayerPrefs.SetString(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN, value);
			}
		}

		public Action<Action,Action<Error>> OnInvalidToken;

		public void CheckInvalidToken(Error error, Action<Error> onErrorCallback, Action repeatCall)
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
