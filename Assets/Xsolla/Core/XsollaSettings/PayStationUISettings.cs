using System;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUISettings
	{
		public bool isFoldout;
		public PaystationTheme paystationTheme = PaystationTheme.PS4_DefaultDark;
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
			var ui = new PayStationUI{
				theme = ConvertToString(paystationTheme)
			};

			if (paystationSize != PaystationSize.Auto)
				ui.size = ConvertToString(paystationSize);

			if (paystationVersion != PaystationVersion.Auto)
				ui.version = ConvertToString(paystationVersion);

			return ui;
		}

		private string ConvertToString(PaystationTheme theme)
		{
			switch (theme)
			{
				case PaystationTheme.Default:		return "default";
				case PaystationTheme.Dark:			return "dark";
				case PaystationTheme.DefaultDark:	return "default_dark";
				case PaystationTheme.PS4_DefaultLight:	return "ps4-default-light";
				case PaystationTheme.PS4_DefaultDark:	return "ps4-default-dark";
				default: goto case PaystationTheme.PS4_DefaultDark;
			}
		}

		private string ConvertToString(PaystationSize size)
		{
			switch (size)
			{
				case PaystationSize.Small:	return "small";
				case PaystationSize.Medium:	return "medium";
				case PaystationSize.Large:	return "large";
				default: goto case PaystationSize.Medium;
			}
		}

		private string ConvertToString(PaystationVersion version)
		{
			switch (version)
			{
				case PaystationVersion.Desktop:	return "desktop";
				case PaystationVersion.Mobile:	return "mobile";
				default: goto case PaystationVersion.Desktop;
			}
		}

		public enum PaystationTheme
		{
			Default,
			Dark,
			DefaultDark,
			PS4_DefaultLight,
			PS4_DefaultDark
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

		public enum PaystationEnvironment
		{
			Desktop,
			Android,
			WebGL
		}
	}
}
