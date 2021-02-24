using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class FontsDrawer : BaseDrawer
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
				EditorGUILayout.LabelField("Fonts", EditorStyles.centeredGreyMiniLabel);
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

			var props = ThemesLibrary.Current?.Fonts;
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

				var themes = ThemesLibrary.Themes;
				foreach (var theme in themes)
				{
					var prop = theme.Fonts.First(x => x.Id == item.Id);
					prop.Font = (Font) EditorGUILayout.ObjectField(prop.Font, typeof(Font), false);
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
				ThemesManager.ChangeFontName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += offsetPerField;

			foreach (var theme in themes)
			{
				var prop = theme.GetFontProperty(item.Id);
				prop.Font = (Font) EditorGUI.ObjectField(fieldRect, prop.Font, typeof(Font), false);
				fieldRect.x += offsetPerField;
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Fonts");
		}

		private void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeFontsOrder(oldIndex, newIndex);
			IsMetaDirty = true;
		}

		private void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateFont();
			IsMetaDirty = true;
		}

		private void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			ThemesManager.DeleteFont(item.Id);
			IsMetaDirty = true;
		}
	}
}