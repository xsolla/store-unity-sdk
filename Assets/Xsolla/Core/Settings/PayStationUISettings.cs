using System;
using UnityEngine;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUISettings
	{
		public bool isFoldout = true;
		public string paystationThemeId = "63295aab2e47fab76f7708e3";

		public static PayStationUI GenerateSettings()
		{
#if UNITY_ANDROID
			return XsollaSettings.AndroidPayStationUISettings.CreateSettings();
#elif UNITY_WEBGL
			return XsollaSettings.WebglPayStationUISettings.CreateSettings();
#elif UNITY_IOS
			return XsollaSettings.IosPayStationUISettings.CreateSettings();
#else
			return XsollaSettings.DesktopPayStationUISettings.CreateSettings();
#endif
		}

		private PayStationUI CreateSettings()
		{
			var settings = new PayStationUI {
				theme = paystationThemeId,
				is_independent_windows = true
			};

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
			settings.is_independent_windows = null;
#endif
			return settings;
		}
	}
}