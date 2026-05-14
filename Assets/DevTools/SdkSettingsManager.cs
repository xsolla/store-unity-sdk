using UnityEditor;
using Xsolla.Core;

namespace Xsolla.DevTools
{
	public static class SdkSettingsManager
	{
		[MenuItem("Dev Tools/Reset SDK Settings for Xsolla Demo", false, 1010)]
		public static void ResetSdkSettingsForXsollaDemo()
		{
			ResetSettings();
		}

		[MenuItem("Dev Tools/Reset SDK Settings for Tech Demo", false, 1020)]
		public static void ResetSdkSettingsForTechDemo()
		{
			ResetSettings("77640");
		}

		private static void ResetSettings(string storeId = null)
		{
			XsollaSettings.StoreProjectId = string.IsNullOrEmpty(storeId)
				? Constants.DEFAULT_PROJECT_ID
				: storeId;

			XsollaSettings.LoginId = Constants.DEFAULT_LOGIN_ID;
			XsollaSettings.OAuthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;
			XsollaSettings.CallbackUrl = string.Empty;
			XsollaSettings.InAppBrowserEnabled = true;
			XsollaSettingsEditor.SaveSettingsAsset();
		}
	}
}