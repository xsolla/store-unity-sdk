using UnityEditor;
using UnityEngine;

namespace Xsolla.Demo
{
	[CustomEditor(typeof(DemoSettings))]
	public partial class DemoSettingsEditor : Editor
	{
		[MenuItem("Window/Xsolla/Demo Settings", false, 1010)]
		private static void SelectDemoSettingsAsset()
		{
			Selection.activeObject = DemoSettings.Instance;
		}

		private GUIStyle GroupAreaStyle => GUI.skin.box;

		private GUIStyle GroupHeaderStyle => EditorStyles.boldLabel;

		public override void OnInspectorGUI()
		{
			GeneralSettings();
			PlatformSettings();
			SteamSettings();
		}
	}
}