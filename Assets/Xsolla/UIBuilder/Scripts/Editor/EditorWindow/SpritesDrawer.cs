using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class SpritesDrawer : BaseDrawer
	{
		private float SpriteFieldSize => EditorGUIUtility.singleLineHeight * 3f;

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
				EditorGUILayout.LabelField("Sprites", EditorStyles.centeredGreyMiniLabel);
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

			var props = ThemesLibrary.Current?.Sprites;
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
			MetaList.elementHeight = SpriteFieldSize;

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
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();

					var prop = theme.Sprites.First(x => x.Id == item.Id);
					prop.Sprite = (Sprite) EditorGUILayout.ObjectField(prop.Sprite, typeof(Sprite), false, GUILayout.Height(SpriteFieldSize), GUILayout.Width(SpriteFieldSize));

					EditorGUILayout.Space();
					EditorGUILayout.EndHorizontal();
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
				ThemesManager.ChangeSpriteName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += offsetPerField;

			fieldRect.x += (fieldRect.width - SpriteFieldSize) / 2f;
			fieldRect.height = SpriteFieldSize;
			fieldRect.width = SpriteFieldSize;

			foreach (var theme in themes)
			{
				var prop = theme.GetSpriteProperty(item.Id);
				prop.Sprite = (Sprite) EditorGUI.ObjectField(fieldRect, prop.Sprite, typeof(Sprite), false);
				fieldRect.x += offsetPerField;
			}
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Sprites");
		}

		private void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeSpritesOrder(oldIndex, newIndex);
			IsMetaDirty = true;
		}

		private void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateSprite();
			IsMetaDirty = true;
		}

		private void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			ThemesManager.DeleteSprite(item.Id);
			IsMetaDirty = true;
		}
	}
}