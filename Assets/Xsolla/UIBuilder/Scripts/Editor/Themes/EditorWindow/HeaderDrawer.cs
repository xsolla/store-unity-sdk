using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class HeaderDrawer : BaseDrawer
	{
		public override void Draw(ThemeEditorWindow window)
		{
			EditorGUILayout.Space();

			var guiColor = GUI.color;
			GUI.color = window.IsEditMode ? Color.red : guiColor;

			if (GUILayout.Button("Edit Mode"))
			{
				window.IsEditMode = !window.IsEditMode;
			}

			if (!window.IsEditMode && ThemesLibrary.Themes.Count == 0)
			{
				EditorGUILayout.HelpBox("You don't have any themes yet.\nTurn on Edit Mode and add first theme", MessageType.Info);
			}

			GUI.color = guiColor;
		}
	}
}