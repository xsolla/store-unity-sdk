#if UNITY_IOS
using System;

namespace Xsolla.Core
{
	internal static class XsollaWebBrowserHandlerIos
	{
		public static void OpenPayStation(string token, Action<BrowserCloseInfo> onBrowserClosed)
		{
			new IosPayments().Perform(
				token,
				isClosedManually => {
					var info = new BrowserCloseInfo {
						isManually = isClosedManually
					};
					onBrowserClosed?.Invoke(info);
				});
		}
	}
}
#endif