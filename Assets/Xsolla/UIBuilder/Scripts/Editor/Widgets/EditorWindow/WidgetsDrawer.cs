using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsDrawer : PropertiesMetaDrawer
	{
		public void Draw()
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

			var widgets = WidgetsLibrary.Widgets;
			foreach (var widget in widgets)
			{
				MetaItems.Add(new PropertyMeta(widget.Id, widget.Name));
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
			var fieldRect = new Rect
			(
				rect.x + EditorGUIUtility.standardVerticalSpacing,
				rect.y + EditorGUIUtility.standardVerticalSpacing,
				rect.width / 2 - EditorGUIUtility.standardVerticalSpacing * 2,
				rect.height = EditorGUIUtility.singleLineHeight
			);

			var item = MetaItems[index];
			var widget = WidgetsLibrary.Widgets.First(x => x.Id == item.Id);

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				WidgetsManager.ChangeWidgetName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += rect.width / 2;

			var prefab = EditorGUI.ObjectField(fieldRect, widget.Prefab, typeof(GameObject), false) as GameObject;
			if (prefab != widget.Prefab)
			{
				widget.Prefab = prefab;
				IsMetaDirty = true;
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Widgets");
		}

		private void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			WidgetsManager.ChangeWidgetsOrder(oldIndex, newIndex);
			IsMetaDirty = true;
		}

		private void OnAddElement(ReorderableList list)
		{
			WidgetsManager.CreateWidget();
			IsMetaDirty = true;
		}

		private void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			WidgetsManager.DeleteWidget(item.Id);
			IsMetaDirty = true;
		}
	}
}