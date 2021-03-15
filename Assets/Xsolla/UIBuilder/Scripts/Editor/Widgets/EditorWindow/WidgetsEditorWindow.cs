using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsEditorWindow : EditorWindow
	{
		private readonly WidgetsDrawer WidgetsDrawer = new WidgetsDrawer();

		private Vector2 ScrollPosition { get; set; }

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 1f;

			if (GUILayout.Button("Refresh"))
			{
				EditorUtility.SetDirty(WidgetsLibrary.Instance);
				HandleSceneObjects();
				HandleAssets();
			}

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

			EditorGUILayout.Space();
			WidgetsDrawer.Draw();

			EditorGUILayout.EndScrollView();
			EditorGUIUtility.labelWidth = 0;
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