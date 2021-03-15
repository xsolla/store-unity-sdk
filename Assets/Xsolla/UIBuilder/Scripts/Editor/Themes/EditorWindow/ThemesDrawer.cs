using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemesDrawer : PropertiesMetaDrawer
	{
		public void Draw(ThemeEditorWindow window)
		{
			if (IsMetaDirty)
			{
				RefreshMeta();
				IsMetaDirty = false;
			}

			MetaList.DoLayoutList();
		}

		private void RefreshMeta()
		{
			MetaItems = new List<PropertyMeta>();

			var themes = ThemesLibrary.Themes;
			if (themes != null)
			{
				foreach (var theme in themes)
				{
					MetaItems.Add(new PropertyMeta(theme.Id, theme.Name));
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

			MetaList = new ReorderableList(MetaItems, typeof(PropertyMeta));
			MetaList.drawElementCallback += OnDrawElement;
			MetaList.drawHeaderCallback += OnDrawHeader;
			MetaList.onReorderCallbackWithDetails += OnReorderElement;
			MetaList.onAddCallback += OnAddElement;
			MetaList.onRemoveCallback += OnRemoveElement;
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var fieldRect = CalculateFieldRect(rect, 1);
			var item = MetaItems[index];

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				ThemesManager.ChangeThemeName(item.Id, name);
				item.Name = name;
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Themes");
		}

		private void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeThemesOrder(oldIndex, newIndex);
			IsMetaDirty = true;
		}

		private void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateTheme();
			IsMetaDirty = true;
			ThemeEditorWindow.MarkMetaDirty();
		}

		private void OnRemoveElement(ReorderableList list)
		{
			var index = list.index;
			if (index >= 0 && index < MetaItems.Count)
			{
				var item = MetaItems[index];
				ThemesManager.DeleteTheme(item.Id);
			}

			IsMetaDirty = true;
			ThemeEditorWindow.MarkMetaDirty();
		}
	}
}