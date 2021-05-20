using UnityEditor;
using UnityEngine;
using Xsolla.PayStation;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private static bool XsollaPaystationSettings()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("PayStation SDK Settings", EditorStyles.boldLabel);
				XsollaSettings.PaystationTheme = (PaystationTheme)EditorGUILayout.EnumPopup("Paystation theme", XsollaSettings.PaystationTheme);
				XsollaSettings.PayStationTokenRequestUrl = EditorGUILayout.TextField(new GUIContent("Token request URL"),  XsollaSettings.PayStationTokenRequestUrl);
				//XsollaSettings.InAppBrowserEnabled = EditorGUILayout.Toggle("Enable in-app browser?", XsollaSettings.InAppBrowserEnabled);
			}
      
			EditorGUILayout.Space();
            return false;
		}
	}
}

