using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ColorsDrawer : PropertiesMetaDrawer
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

			var props = ThemesLibrary.Current?.Colors;
			if (props != null)
			{
				foreach (var prop in props)
				{
					MetaItems.Add(new PropertyMeta(prop.Id, prop.Name));
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