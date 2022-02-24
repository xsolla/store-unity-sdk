using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private static void XsollaAndroidSettings()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Android Settings", EditorStyles.boldLabel);
				XsollaSettings.UseDeepLinking = EditorGUILayout.Toggle("Use deep linking?", XsollaSettings.UseDeepLinking);
				XsollaSettings.DeepLinkRedirectUrl = EditorGUILayout.TextField(new GUIContent("Redirect URL"),  XsollaSettings.DeepLinkRedirectUrl);
			}

			EditorGUILayout.Space();
		}
	}
}
