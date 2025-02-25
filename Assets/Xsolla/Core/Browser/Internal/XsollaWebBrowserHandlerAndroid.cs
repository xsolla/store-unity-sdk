#if UNITY_ANDROID
using System;

namespace Xsolla.Core
{
	internal static class XsollaWebBrowserHandlerAndroid
	{
		public static void OpenPayStation(string token, Action<BrowserCloseInfo> onBrowserClosed, PlatformSpecificAppearance platformSpecificAppearance)
		{
			new AndroidPayments().Perform(
				token,
				isClosedManually => {
					var info = new BrowserCloseInfo {
						isManually = isClosedManually
					};
					onBrowserClosed?.Invoke(info);
				},
				platformSpecificAppearance?.AndroidActivityType);
		}
	}
}
#endif