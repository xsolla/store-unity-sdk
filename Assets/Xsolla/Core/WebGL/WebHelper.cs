#if UNITY_WEBGL
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class WebHelper
	{
		[DllImport("__Internal")]
		private static extern string GetUserAgent();

		[DllImport("__Internal")]
		public static extern string GetBrowserLanguage();

		public static bool IsBrowserSafari()
		{
			var userAgent = GetUserAgent();

			if (Application.isMobilePlatform)
			{
				return (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
					&& userAgent.Contains("Safari")
					&& !userAgent.Contains("CriOS");
			}

			return userAgent.Contains("Safari")
				&& userAgent.Contains("AppleWebKit")
				&& !userAgent.Contains("Chrome")
				&& !userAgent.Contains("Chromium");
		}
	}
}
#endif