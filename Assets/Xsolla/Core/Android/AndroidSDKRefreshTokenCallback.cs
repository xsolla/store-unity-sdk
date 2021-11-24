using System;
using UnityEngine;

namespace Xsolla.Core
{
	public class AndroidSDKRefreshTokenCallback : AndroidJavaProxy
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
				if (OnSuccess != null)
					OnSuccess.Invoke(token);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("AndroidSDKRefreshTokenCallback.onSuccess: {0}", ex.Message));
				if (OnError != null)
					OnError.Invoke(new Error(errorMessage: ex.Message));
			}
		}

		public void onError(AndroidJavaException throwable, string errorMessage)
		{
			var error = new Error(errorMessage: errorMessage);
			if (OnError != null)
				OnError.Invoke(error);
		}
	}
}
