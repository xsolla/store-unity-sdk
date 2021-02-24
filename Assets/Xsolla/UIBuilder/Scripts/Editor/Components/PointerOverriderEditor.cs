using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class PointerOverriderEditor
	{
		private SerializedProperty PointerEvents { get; }

		private SerializedProperty TransitionMode { get; }

		private SerializedProperty IsOverrideHover { get; }

		private SerializedProperty HoverPropertyId { get; }

		private SerializedProperty IsOverridePress { get; }

		private SerializedProperty PressPropertyId { get; }

		public void DrawGUI(List<string> ids, string[] names, int mainIndex)
		{
			EditorGUILayout.PropertyField(TransitionMode);

			if (TransitionMode.enumValueIndex > 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();

				EditorGUILayout.PropertyField(PointerEvents);
				var canEdit = PointerEvents.objectReferenceValue;

				GUI.enabled = canEdit;
				DrawOverride(IsOverrideHover, HoverPropertyId, "Override on hover", canEdit, ids, names, mainIndex);
				DrawOverride(IsOverridePress, PressPropertyId, "Override on press", canEdit, ids, names, mainIndex);
				GUI.enabled = true;

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		private static void DrawOverride(SerializedProperty isOverride, SerializedProperty propertyId, string label, bool canEdit, List<string> ids, string[] names, int mainIndex)
		{
			EditorGUILayout.BeginHorizontal();
			isOverride.boolValue = EditorGUILayout.ToggleLeft(label, isOverride.boolValue, GUILayout.Width(EditorGUIUtility.labelWidth));

			if (canEdit && isOverride.boolValue)
			{
				var index = ids.IndexOf(propertyId.stringValue);
				var selectedIndex = EditorGUILayout.Popup(index, names);
				if (selectedIndex != index)
				{
					propertyId.stringValue = ids[selectedIndex];
				}
			}
			else
			{
				GUI.enabled = false;
				EditorGUILayout.LabelField(names[mainIndex]);
				GUI.enabled = canEdit;
			}

			EditorGUILayout.EndHorizontal();
		}

		public PointerOverriderEditor(SerializedObject serializedObject)
		{
			var overrider = serializedObject.FindProperty("_pointerOverrider");
			PointerEvents = overrider.FindPropertyRelative("_pointerEvents");
			TransitionMode = overrider.FindPropertyRelative("_transitionMode");

			IsOverrideHover = overrider.FindPropertyRelative("_isOverrideHover");
			HoverPropertyId = overrider.FindPropertyRelative("_hoverPropertyId");

			IsOverridePress = overrider.FindPropertyRelative("_isOverridePress");
			PressPropertyId = overrider.FindPropertyRelative("_pressPropertyId");
		}
	}
}