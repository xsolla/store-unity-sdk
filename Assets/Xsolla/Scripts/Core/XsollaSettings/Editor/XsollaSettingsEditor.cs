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
			var changed = XsollaLoginSettings();

			changed = changed || PublishingPlatformSettings();

			changed = changed || XsollaStoreSettings();

			XsollaPaystationSettings();

			if (changed)
			{
				XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
			}
		}
	}
}

