using System.Linq;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(TextColorDecorator))]
	public class TextColorEditor : ThemeDecoratorEditor
	{
		private SerializedProperty textProp;

		protected override void FindProperties()
		{
			textProp = serializedObject.FindProperty("_text");
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(textProp);

			var colors = ThemesLibrary.Current.Colors.ToArray();
			var guids = colors.Select(x => x.Id).ToList();
			var index = guids.IndexOf(propId.stringValue);
			var names = colors.Select(x => x.Name).ToArray();

			var selectedIndex = EditorGUILayout.Popup("Color", index, names);
			if (selectedIndex != index)
			{
				propId.stringValue = guids[selectedIndex];
			}
		}
	}
}