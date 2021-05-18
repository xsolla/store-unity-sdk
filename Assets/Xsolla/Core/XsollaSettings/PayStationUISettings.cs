using System;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUISettings
	{
		public bool isFoldout;
		public bool isOverride;
		public PaystationTheme paystationTheme = PaystationTheme.PS4_DefaultDark;
		public PaystationSize paystationSize = PaystationSize.Medium;
		public PaystationVersion paystationVersion = PaystationVersion.Desktop;

		public static PayStationUI GenerateSettings()
		{
#if UNITY_ANDROID
			if (XsollaSettings.AndroidPayStationUISettings.isOverride)
			{
				return XsollaSettings.AndroidPayStationUISettings.CreateSettings();
			}
#elif UNITY_WEBGL
			if (XsollaSettings.WebglPayStationUISettings.isOverride)
			{
				return XsollaSettings.WebglPayStationUISettings.CreateSettings();
			}
#else
			if (XsollaSettings.DesktopPayStationUISettings.isOverride)
			{
				return XsollaSettings.DesktopPayStationUISettings.CreateSettings();
			}
#endif
			//Pay Station will define the settings depending on the platform.
			return null;
		}

		private PayStationUI CreateSettings()
		{
			return new PayStationUI
			{
				theme = ConvertToString(paystationTheme),
				size = ConvertToString(paystationSize),
				version = ConvertToString(paystationVersion)
			};
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
				default: goto case PaystationTheme.Dark;
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
			Small,
			Medium,
			Large
		}

		public enum PaystationVersion
		{
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
