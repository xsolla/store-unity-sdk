using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string PACK_IN_APP_BROWSER_LABEL = "Pack In-App Browser in Build";
		private const string PACK_IN_APP_BROWSER_TOOLTIP = "If enabled, the build of your application includes all in-app browser files. " +
			"If not, the latest version of browser files (300 MB) is downloaded when the user first opens an in-app browser.";

		private bool DesktopSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Desktop", GroupHeaderStyle);

			var changed = false;

			if (XsollaSettings.InAppBrowserEnabled)
			{
				var packInAppBrowserInBuild = EditorGUILayout.Toggle(new GUIContent(PACK_IN_APP_BROWSER_LABEL, PACK_IN_APP_BROWSER_TOOLTIP), XsollaSettings.PackInAppBrowserInBuild);
				if (packInAppBrowserInBuild != XsollaSettings.PackInAppBrowserInBuild)
				{
					XsollaSettings.PackInAppBrowserInBuild = packInAppBrowserInBuild;
					changed = true;
				}
			}
			else
			{
				GUI.enabled = false;
				EditorGUILayout.Toggle(new GUIContent(PACK_IN_APP_BROWSER_LABEL, PACK_IN_APP_BROWSER_TOOLTIP), false);
				GUI.enabled = true;
			}

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}