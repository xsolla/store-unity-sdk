using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	[CustomEditor(typeof(XsollaSettings))]
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		[MenuItem("Window/Xsolla/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = XsollaSettings.Instance;
		}

		public override void OnInspectorGUI()
		{
			var changed =	XsollaLoginSettings() ||
							PublishingPlatformSettings() ||
							XsollaStoreSettings() ||
							AndroidSDKSettings() ||
							InventorySDKSettings();

			XsollaAndroidSettings();
			DemoSettings();

			if (changed)
			{
				DeleteRecord(Constants.LAST_SUCCESS_AUTH_TOKEN);
				DeleteRecord(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);
				DeleteRecord(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME);
			}
		}

		private void DeleteRecord(string key)
		{
			if (!string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key))
				PlayerPrefs.DeleteKey(key);
		}
	}
}
