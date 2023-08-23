using System;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUISettings
	{
		public bool isFoldout;
		public string paystationThemeId = "63295a9a2e47fab76f7708e1";
		public PaystationSize paystationSize = PaystationSize.Auto;
		public PaystationVersion paystationVersion = PaystationVersion.Auto;

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
			var ui = new PayStationUI {
				theme = paystationThemeId
			};

			if (paystationSize != PaystationSize.Auto)
				ui.size = ConvertToString(paystationSize);

			if (paystationVersion != PaystationVersion.Auto)
				ui.version = ConvertToString(paystationVersion);

			return ui;
		}

		private string ConvertToString(PaystationSize size)
		{
			switch (size)
			{
				case PaystationSize.Small: return "small";
				case PaystationSize.Medium: return "medium";
				case PaystationSize.Large: return "large";
				default: goto case PaystationSize.Medium;
			}
		}

		private string ConvertToString(PaystationVersion version)
		{
			switch (version)
			{
				case PaystationVersion.Desktop: return "desktop";
				case PaystationVersion.Mobile: return "mobile";
				default: goto case PaystationVersion.Desktop;
			}
		}

		public enum PaystationSize
		{
			Auto,
			Small,
			Medium,
			Large
		}

		public enum PaystationVersion
		{
			Auto,
			Desktop,
			Mobile
		}
	}
}