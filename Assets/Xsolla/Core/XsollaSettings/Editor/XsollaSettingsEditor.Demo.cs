using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private void DemoSettings()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Demo Settings", EditorStyles.boldLabel);

				var debugPrintType = (LogLevel)EditorGUILayout.Popup(
					label: "Print to Console",
					selectedIndex: (int)XsollaSettings.LogLevel,
					displayedOptions: new string[] { "Info, Warnings and Errors", "Warnings and Errors", "Only Errors" });
				if (debugPrintType != XsollaSettings.LogLevel)
				{
					XsollaSettings.LogLevel = debugPrintType;
				}
			}
			EditorGUILayout.Space();
		}
	}
}
