#if UNITY_STANDALONE
namespace Xsolla.Core
{
	internal class SteamOverlayBrowserHandler
	{
		public static void OpenPayStation(string token, SdkType sdkType)
		{
			var payStationUrl = new PayStationUrlBuilder(token, sdkType).Build();
			SteamUtils.OpenUrlInOverlay(payStationUrl);
		}
	}
}
#endif