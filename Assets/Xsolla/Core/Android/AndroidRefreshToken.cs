using System;

namespace Xsolla.Core
{
	internal class AndroidRefreshToken
	{
		public void Perform(Action onSuccess, Action<Error> onError)
		{
			var androidHelper = new AndroidHelper();

			var canRefresh = androidHelper.Xlogin.CallStatic<bool>("canRefreshToken");
			if (!canRefresh)
			{
				XDebug.Log("Android social token refresh is not available at the moment");
				onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed));
				return;
			}

			var callback = new AndroidRefreshTokenCallback(
				token => androidHelper.MainThreadExecutor.Enqueue(() =>
				{
					XsollaToken.Create(token);
					onSuccess?.Invoke();
				}),
				error => androidHelper.MainThreadExecutor.Enqueue(() => onError(error)));

			androidHelper.Xlogin.CallStatic("refreshToken", callback);
		}
	}
}