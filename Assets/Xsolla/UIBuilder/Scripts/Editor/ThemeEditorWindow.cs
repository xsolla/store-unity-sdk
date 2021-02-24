using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemeEditorWindow : EditorWindow
	{
		private readonly BaseDrawer HeaderDrawer = new HeaderDrawer();

		private readonly BaseDrawer ThemesDrawer = new ThemesDrawer();

		private readonly BaseDrawer ColorsDrawer = new ColorsDrawer();

		private readonly BaseDrawer SpritesDrawer = new SpritesDrawer();

		private readonly BaseDrawer FontsDrawer = new FontsDrawer();

		private Vector2 ScrollPosition { get; set; }

		public float PropLabelsWidth { get; set; }

		public bool IsEditMode { get; set; }

		private void OnGUI()
		{
			var themes = ThemesLibrary.Themes;
			PropLabelsWidth = position.width / (themes.Count + 2);

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
			EditorGUIUtility.labelWidth = 1f;

			EditorGUILayout.Space();
			HeaderDrawer.Draw(this);

			EditorGUILayout.Space();
			ThemesDrawer.Draw(this);

			EditorGUILayout.Space();
			ColorsDrawer.Draw(this);

			EditorGUILayout.Space();
			SpritesDrawer.Draw(this);

			EditorGUILayout.Space();
			FontsDrawer.Draw(this);

			EditorGUIUtility.labelWidth = 0;
			EditorGUILayout.EndScrollView();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(ThemesLibrary.Instance);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				Repaint();
			}
		}

		public static void MarkMetaDirty()
		{
			var window = GetWindow<ThemeEditorWindow>();
			window.HeaderDrawer.IsMetaDirty = true;
			window.ThemesDrawer.IsMetaDirty = true;
			window.ColorsDrawer.IsMetaDirty = true;
			window.SpritesDrawer.IsMetaDirty = true;
		}

		[MenuItem("Window/Xsolla/UI Themes Editor")]
		private static void OpenWindow()
		{
			GetWindow<ThemeEditorWindow>("UI Themes Editor");
		}
	}
}