using UnityEditor;
using Xsolla.Login;

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
			var changed =   XsollaLoginSettings() ||
                            PublishingPlatformSettings() ||
			                XsollaStoreSettings() ||
                            XsollaPaystationSettings() ||
                            AndroidSDKSettings() ||
							InventorySDKSettings();

			XsollaAndroidSettings();

			if (changed)
			{
				XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
				XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);
				XsollaLogin.Instance.DeleteToken(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME);
			}
		}
	}
}

