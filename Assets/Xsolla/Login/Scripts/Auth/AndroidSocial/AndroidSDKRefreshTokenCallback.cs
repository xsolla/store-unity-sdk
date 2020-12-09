using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class AndroidSDKRefreshTokenCallback : AndroidJavaProxy, IStoreStringAction
	{
		public Action<string> OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }

		public AndroidSDKRefreshTokenCallback() : base("com.xsolla.android.login.callback.RefreshTokenCallback") { }

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
				Debug.LogError($"AndroidSDKRefreshTokenCallback.onSuccess: {ex.Message}");
				OnError?.Invoke(new Error(errorMessage: ex.Message));
			}
		}

		public void onError(AndroidJavaException throwable, string errorMessage)
		{
			var error = new Error(errorMessage: errorMessage);
			OnError?.Invoke(error);
		}
	}
}
