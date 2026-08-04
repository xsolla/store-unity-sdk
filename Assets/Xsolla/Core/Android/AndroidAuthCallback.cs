#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidAuthCallback : AndroidJavaProxy
	{
		private readonly AndroidHelper AndroidHelper;
		private readonly Action OnSuccess;
		private readonly Action<Error> OnError;
		private readonly Action OnCancel;

		public AndroidAuthCallback(AndroidHelper androidHelper, Action onSuccess, Action<Error> onError, Action onCancel) : base("com.xsolla.android.bridge.login.callback.AuthCallback")
		{
			AndroidHelper = androidHelper;
			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;
		}

		// called from com.xsolla.android.bridge's AuthFlowCoordinator once the auth deep link redirect is dispatched
		public void onSuccess()
		{
			try
			{
				var accessToken = AndroidHelper.Xlogin.CallStatic<string>("getToken");
				var refreshToken = AndroidHelper.Xlogin.CallStatic<string>("getRefreshToken");
				AndroidHelper.MainThreadExecutor.Enqueue(() => {
					XsollaToken.Create(accessToken, refreshToken);
					OnSuccess?.Invoke();
				});
			}
			catch (Exception e)
			{
				AndroidHelper.MainThreadExecutor.Enqueue(() => OnError?.Invoke(new Error(errorMessage: e.Message)));
			}
		}

		// called from com.xsolla.android.bridge's AuthFlowCoordinator once the auth deep link redirect is dispatched
		public void onError(AndroidJavaObject _, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "CANCELLED")
			{
				AndroidHelper.MainThreadExecutor.Enqueue(() => OnCancel?.Invoke());
			}
			else
			{
				AndroidHelper.MainThreadExecutor.Enqueue(() => OnError?.Invoke(new Error(errorMessage: errorMessage)));
			}
		}
	}
}
#endif