using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class PropertiesMetaDrawer
	{
		public bool IsMetaDirty { get; set; } = true;

		protected List<PropertyMeta> MetaItems { get; set; }

		protected ReorderableList MetaList { get; set; }

		protected Rect CalculateFieldRect(Rect lineRect, int elementsCount)
		{
			return new Rect
			(
				lineRect.x + EditorGUIUtility.standardVerticalSpacing,
				lineRect.y + EditorGUIUtility.standardVerticalSpacing,
				lineRect.width / elementsCount - EditorGUIUtility.standardVerticalSpacing * 2,
				lineRect.height = EditorGUIUtility.singleLineHeight
			);
		}

		protected float CalculateOffsetPerField(Rect rect)
		{
			return rect.width + EditorGUIUtility.standardVerticalSpacing * 2;
		}
	}
}