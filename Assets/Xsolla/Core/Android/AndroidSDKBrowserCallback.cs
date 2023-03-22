using System;
using UnityEngine;

namespace Xsolla.Core.Android
{
	internal class AndroidSDKBrowserCallback : AndroidJavaProxy
	{
		public Action<bool> OnBrowserClosed { get; set; }

		public AndroidSDKBrowserCallback() : base("com.xsolla.android.payments.callback.BrowserCallback") { }

		public void onBrowserClosed(bool isManually)
		{
			OnBrowserClosed?.Invoke(isManually);
		}
	}
}