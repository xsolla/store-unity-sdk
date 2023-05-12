using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidRefreshTokenCallback : AndroidJavaProxy
	{
		private readonly Action<string> OnSuccess;
		private readonly Action<Error> OnError;

		public AndroidRefreshTokenCallback(Action<string> onSuccess, Action<Error> onError) : base("com.xsolla.android.login.callback.RefreshTokenCallback")
		{
			OnSuccess = onSuccess;
			OnError = onError;
		}

		public void onSuccess()
		{
			try
			{
				var token = new AndroidHelper().Xlogin.CallStatic<string>("getToken");
				OnSuccess?.Invoke(token);
			}
			catch (Exception ex)
			{
				XDebug.LogError($"AndroidSDKRefreshTokenCallback.onSuccess: {ex.Message}");
				OnError?.Invoke(new Error(errorMessage: ex.Message));
			}
		}

		public void onError(AndroidJavaObject _, string errorMessage)
		{
			var error = new Error(errorMessage: errorMessage);
			OnError?.Invoke(error);
		}
	}
}