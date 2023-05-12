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

		private GUIStyle GroupAreaStyle => GUI.skin.box;

		private GUIStyle GroupHeaderStyle => EditorStyles.boldLabel;

		public override void OnInspectorGUI()
		{
			var changed = AutoFillSettings() ||
			              GeneralSettings() ||
			              LoginSettings() ||
			              PayStationSettings() ||
			              AndroidSettings() ||
			              DesktopSettings() ||
			              EditorSettings();

			if (changed)
				DropSavedTokens();
		}

		public static void DropSavedTokens()
		{
			XsollaToken.DeleteSavedInstance();
		}

		private static void DrawErrorBox(string message)
		{
			EditorGUILayout.HelpBox(message, MessageType.Error, true);
			EditorGUILayout.Space();
		}
	}
}