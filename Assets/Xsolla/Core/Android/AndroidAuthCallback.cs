using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidAuthCallback : AndroidJavaProxy
	{
		private readonly Action<string> OnSuccess;
		private readonly Action<Error> OnError;
		private readonly Action OnCancel;

		public AndroidAuthCallback(Action<string> onSuccess, Action<Error> onError, Action onCancel) : base("com.xsolla.android.login.callback.AuthCallback")
		{
			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;
		}

		public void onSuccess()
		{
			try
			{
				var token = new AndroidHelper().Xlogin.CallStatic<string>("getToken");
				OnSuccess?.Invoke(token);
			}
			catch (Exception e)
			{
				OnError?.Invoke(new Error(errorMessage: e.Message));
			}
		}

		public void onError(AndroidJavaObject _, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "CANCELLED")
			{
				OnCancel?.Invoke();
			}
			else
			{
				OnError?.Invoke(new Error(errorMessage: errorMessage));
			}
		}
	}
}