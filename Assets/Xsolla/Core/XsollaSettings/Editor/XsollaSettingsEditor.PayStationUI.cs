using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private bool XsollaPayStationUISettings()
		{
			var changed = false;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Pay Station UI Settings", EditorStyles.boldLabel);

				DrawPayStationUISettings(XsollaSettings.DesktopPayStationUISettings, "Desktop build UI settings");
				DrawPayStationUISettings(XsollaSettings.WebglPayStationUISettings, "WebGL build UI settings");
				DrawPayStationUISettings(XsollaSettings.AndroidPayStationUISettings, "Android build UI settings");
			}
			EditorGUILayout.Space();

			return changed;
		}

		private void DrawPayStationUISettings(PayStationUISettings settings, string title)
		{
			EditorGUI.indentLevel++;

			settings.isFoldout = EditorGUILayout.Foldout(settings.isFoldout, title);
			if (settings.isFoldout)
			{
				settings.paystationTheme = (PayStationUISettings.PaystationTheme)EditorGUILayout.EnumPopup("Pay Station theme", settings.paystationTheme);
				settings.paystationSize = (PayStationUISettings.PaystationSize)EditorGUILayout.EnumPopup("Pay Station size", settings.paystationSize);
				settings.paystationVersion = (PayStationUISettings.PaystationVersion)EditorGUILayout.EnumPopup("Pay Station version", settings.paystationVersion);
				settings.isIndependentWindows = EditorGUILayout.Toggle("Is independent window", settings.isIndependentWindows);
			}

			EditorGUI.indentLevel--;
		}
	}
}
