using System;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUISettings
	{
		public bool isFoldout = true;

		public string paystationThemeId = "63295aab2e47fab76f7708e3";

		public static PayStationUI GenerateSettings()
		{
			var settings = new PayStationUI {
				theme = GePlatformSpecificSettings().paystationThemeId
			};

#if UNITY_EDITOR || UNITY_STANDALONE
			if (XsollaSettings.InAppBrowserEnabled)
				settings.is_independent_windows = true;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
			if (!string.IsNullOrEmpty(XsollaSettings.ApplePayMerchantDomain))
				settings.is_independent_windows = null;
#endif

			return settings;
		}

		private static PayStationUISettings GePlatformSpecificSettings()
		{
#if UNITY_ANDROID
			return XsollaSettings.AndroidPayStationUISettings;
#elif UNITY_WEBGL
			return XsollaSettings.WebglPayStationUISettings;
#elif UNITY_IOS
			return XsollaSettings.IosPayStationUISettings;
#else
			return XsollaSettings.DesktopPayStationUISettings;
#endif
		}
	}
}