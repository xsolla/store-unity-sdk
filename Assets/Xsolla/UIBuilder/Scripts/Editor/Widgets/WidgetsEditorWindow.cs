using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsEditorWindow : EditorWindow
	{
		private readonly WidgetsListDrawer WidgetsDrawer = new WidgetsListDrawer();

		private void OnGUI()
		{
			EditorGUILayout.Space();
			WidgetsDrawer.Draw(this);

			if (GUILayout.Button("Refresh"))
			{
				EditorUtility.SetDirty(WidgetsLibrary.Instance);
				HandleSceneObjects();
				HandleAssets();
			}
		}

		private void HandleSceneObjects()
		{
			var containers = FindObjectsOfType<WidgetContainer>();
			foreach (var container in containers)
			{
				if (!PrefabUtility.IsPartOfAnyPrefab(container))
				{
					WidgetsHandler.Handle(container);
				}
			}
		}

		private void HandleAssets()
		{
			var guids = AssetDatabase.FindAssets(
				"t:prefab",
				new[]
				{
					"Assets"
				}
			);

			var containers = new List<WidgetContainer>();
			foreach (var guid in guids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var container = AssetDatabase.LoadAssetAtPath<WidgetContainer>(assetPath);
				if (container)
				{
					containers.Add(container);
				}
			}

			foreach (var container in containers)
			{
				WidgetsHandler.Handle(container);
			}
		}

		[MenuItem("Window/Xsolla/UI Widgets Editor")]
		private static void OpenWindow()
		{
			GetWindow<WidgetsEditorWindow>("UI Widgets Editor");
		}
	}
}