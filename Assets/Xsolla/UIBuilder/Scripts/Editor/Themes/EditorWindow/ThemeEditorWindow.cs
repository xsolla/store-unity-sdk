using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemeEditorWindow : EditorWindow
	{
		private readonly HeaderDrawer HeaderDrawer = new HeaderDrawer();

		private readonly ThemesDrawer ThemesDrawer = new ThemesDrawer();

		private readonly ColorsDrawer ColorsDrawer = new ColorsDrawer();

		private readonly SpritesDrawer SpritesDrawer = new SpritesDrawer();

		private readonly FontsDrawer FontsDrawer = new FontsDrawer();

		private Vector2 ScrollPosition { get; set; }

		public float PropLabelsWidth { get; private set; }

		private void OnGUI()
		{
			var themes = ThemesLibrary.Themes;
			PropLabelsWidth = position.width / (themes.Count + 1);

			EditorGUIUtility.labelWidth = 1f;

			EditorGUILayout.Space();
			HeaderDrawer.Draw(this);

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

			EditorGUILayout.Space();
			ThemesDrawer.Draw(this);

			EditorGUILayout.Space();
			ColorsDrawer.Draw(this);

			EditorGUILayout.Space();
			SpritesDrawer.Draw(this);

			EditorGUILayout.Space();
			FontsDrawer.Draw(this);

			EditorGUILayout.EndScrollView();
			EditorGUIUtility.labelWidth = 0;

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