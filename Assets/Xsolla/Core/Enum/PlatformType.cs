namespace Xsolla.Core
{
	public enum PlatformType
	{
		None,
		Xsolla,
		GooglePlay,
		AppStore,
		PlaystationNetwork,
		XboxLive,
		NintendoShop,
		Android_Standalone,
		iOS_Standalone,
		PC_Standalone,
		Android_Other,
		iOS_Other,
		PC_Other
	}

	public static class PlatformExtensions
	{
		public static string GetString(this PlatformType platform)
		{
			switch(platform) {
				case PlatformType.None: return string.Empty;
				case PlatformType.Xsolla: return Constants.Platform.XSOLLA;
				case PlatformType.AppStore: return Constants.Platform.APP_STORE;
				case PlatformType.GooglePlay: return Constants.Platform.GOOGLE_PLAY;
				case PlatformType.NintendoShop: return Constants.Platform.NINTENDO_SHOP;
				case PlatformType.PlaystationNetwork: return Constants.Platform.PLAYSTATION_NETWORK;
				case PlatformType.XboxLive: return Constants.Platform.XBOX_LIVE;
				case PlatformType.Android_Standalone: return Constants.Platform.ANDROID_STANDALONE;
				case PlatformType.iOS_Standalone: return Constants.Platform.IOS_STANDALONE;
				case PlatformType.PC_Standalone: return Constants.Platform.PC_STANDALONE;
				case PlatformType.Android_Other: return Constants.Platform.ANDROID_OTHER;
				case PlatformType.iOS_Other: return Constants.Platform.IOS_OTHER;
				case PlatformType.PC_Other: return Constants.Platform.PC_OTHER;
				default: return Constants.Platform.PC_OTHER;
			}
		}
	}
}
