using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private bool XsollaStoreSettings()
		{
			var changed = false;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Store SDK Settings", EditorStyles.boldLabel);
				var projectId = EditorGUILayout.TextField(new GUIContent("Project ID"),  XsollaSettings.StoreProjectId);
				if (projectId != XsollaSettings.StoreProjectId)
				{
					XsollaSettings.StoreProjectId = projectId;
					changed = true;
				}
				var sandbox = EditorGUILayout.Toggle("Enable sandbox?", XsollaSettings.IsSandbox);
				if (sandbox != XsollaSettings.IsSandbox)
				{
					XsollaSettings.IsSandbox = sandbox;
					changed = true;
				}
			}
			EditorGUILayout.Space();
			
			return changed;
		}
	}
}
