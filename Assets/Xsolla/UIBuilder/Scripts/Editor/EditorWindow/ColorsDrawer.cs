using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ColorsDrawer : BaseDrawer
	{
		public override void Draw(ThemeEditorWindow window)
		{
			if (IsMetaDirty)
			{
				RefreshMeta();
				IsMetaDirty = false;
			}

			if (MetaItems.Count > 0)
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Colors", EditorStyles.centeredGreyMiniLabel);
			}

			if (window.IsEditMode)
			{
				MetaList.DoLayoutList();
			}
			else
			{
				DrawProps(window);
			}
		}

		private void RefreshMeta()
		{
			MetaItems = new List<MetaItem>();

			var props = ThemesLibrary.Current?.Colors;
			if (props != null)
			{
				foreach (var prop in props)
				{
					MetaItems.Add(new MetaItem(prop.Id, prop.Name));
				}
			}

			if (MetaList != null)
			{
				MetaList.drawElementCallback -= OnDrawElement;
				MetaList.drawHeaderCallback -= OnDrawHeader;
				MetaList.onReorderCallbackWithDetails -= OnReorderElement;
				MetaList.onAddCallback -= OnAddElement;
				MetaList.onRemoveCallback -= OnRemoveElement;
			}

			MetaList = new ReorderableList(MetaItems, typeof(MetaItem));
			MetaList.drawElementCallback += OnDrawElement;
			MetaList.drawHeaderCallback += OnDrawHeader;
			MetaList.onReorderCallbackWithDetails += OnReorderElement;
			MetaList.onAddCallback += OnAddElement;
			MetaList.onRemoveCallback += OnRemoveElement;
		}

		private void DrawProps(ThemeEditorWindow window)
		{
			foreach (var item in MetaItems)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(item.Name, GUILayout.Width(window.PropLabelsWidth));

				foreach (var theme in ThemesLibrary.Themes)
				{
					var prop = theme.GetColorProperty(item.Id);
					prop.Color = EditorGUILayout.ColorField(prop.Color);
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var themes = ThemesLibrary.Themes;
			var fieldRect = CalculateFieldRect(rect, themes.Count + 1);
			var offsetPerField = CalculateOffsetPerField(fieldRect);

			var item = MetaItems[index];

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				ThemesManager.ChangeColorName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += offsetPerField;

			foreach (var theme in themes)
			{
				var prop = theme.GetColorProperty(item.Id);
				prop.Color = EditorGUI.ColorField(fieldRect, prop.Color);
				fieldRect.x += offsetPerField;
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Colors");
		}

		private void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeColorsOrder(oldIndex, newIndex);
			IsMetaDirty = true;
		}

		private void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateColor();
			IsMetaDirty = true;
		}

		private void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			ThemesManager.DeleteColor(item.Id);
			IsMetaDirty = true;
		}
	}
}