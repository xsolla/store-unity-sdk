using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class HeaderDrawer : PropertiesMetaDrawer
	{
		public void Draw(ThemeEditorWindow window)
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginHorizontal(GUILayout.Width(window.PropLabelsWidth));
			EditorGUILayout.LabelField("Themes");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var guiColor = GUI.color;

			var themes = ThemesLibrary.Themes;
			var current = ThemesLibrary.Current;

			foreach (var theme in themes)
			{
				GUI.color = theme == current ? Color.yellow : guiColor;

				if (GUILayout.Button(theme.Name))
				{
					ThemesLibrary.Current = theme;
				}
			}

			GUI.color = guiColor;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
}