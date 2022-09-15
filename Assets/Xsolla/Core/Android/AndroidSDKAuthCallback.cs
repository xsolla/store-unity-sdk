using System;
using UnityEngine;

namespace Xsolla.Core.Android
{
	public class AndroidSDKAuthCallback : AndroidJavaProxy
	{
		public Action<string> OnSuccess { get; set; }
		public Action OnCancelled { get; set; }
		public Action<Error> OnError { get; set; }

		public AndroidSDKAuthCallback() : base("com.xsolla.android.login.callback.AuthCallback") { }

		public void onSuccess()
		{
			try
			{
				var xlogin = new AndroidJavaClass("com.xsolla.android.login.XLogin");
				var token = xlogin.CallStatic<string>("getToken");
				OnSuccess?.Invoke(token);
			}
			catch (Exception ex)
			{
				Debug.LogError($"AndroidSDKAuthCallback.onSuccess: ERROR {ex.Message}");
				OnError?.Invoke(new Error(errorMessage: ex.Message));
			}
		}

		public void onError(AndroidJavaObject _, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "CANCELLED")
			{
				Debug.LogWarning("AndroidSDKAuthCallback: CANCELLED");
				OnCancelled?.Invoke();
			}
			else
			{
				OnError?.Invoke(new Error(errorMessage: errorMessage));
			}
		}
	}
}