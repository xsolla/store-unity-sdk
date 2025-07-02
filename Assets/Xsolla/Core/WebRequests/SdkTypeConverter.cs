namespace Xsolla.Core
{
	internal static class SdkTypeConverter
	{
		public static string GetSdkTypeAsString(SdkType sdkType)
		{
			return sdkType switch {
				SdkType.Login            => "login",
				SdkType.Store            => "store",
				SdkType.Subscriptions    => "subscriptions",
				SdkType.SettingsFillTool => "settings_fill_tool",
				SdkType.ReadyToUseStore  => "ready_to_use_store",
				_                        => "unknown"
			};
		}
	}
}