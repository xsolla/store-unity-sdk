using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private bool InventorySDKSettings()
		{
			var changed = false;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Inventory SDK Settings", EditorStyles.boldLabel);
				var projectId = EditorGUILayout.TextField(new GUIContent("Web Store URL"),  XsollaSettings.WebStoreUrl);
				if (projectId != XsollaSettings.WebStoreUrl)
				{
					XsollaSettings.WebStoreUrl = projectId;
					changed = true;
				}
			}
			EditorGUILayout.Space();
			
			return changed;
		}
	}
}
