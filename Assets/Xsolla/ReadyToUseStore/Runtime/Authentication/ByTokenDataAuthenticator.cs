using System;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class ByTokenDataAuthenticator : IAuthenticator
	{
		private readonly string AccessToken;
		private readonly string RefreshToken;
		private readonly int ExpiresIn;

		public ByTokenDataAuthenticator(TokenData data)
		{
			if (data == null)
				return;

			AccessToken = data.accessToken;
			RefreshToken = data.refreshToken;
			ExpiresIn = data.expirationTime;
		}

		public void Execute(Action onSuccess, Action<Error> onError, Action onCancel, Action onSkip)
		{
			if (string.IsNullOrEmpty(AccessToken))
			{
				onSkip?.Invoke();
				return;
			}

			XsollaToken.Create(AccessToken, RefreshToken, ExpiresIn);
			onSuccess?.Invoke();
		}
	}
}