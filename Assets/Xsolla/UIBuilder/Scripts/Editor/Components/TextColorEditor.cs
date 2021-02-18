using System.Linq;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(TextColorDecorator))]
	public class TextColorEditor : ThemeDecoratorEditor
	{
		private SerializedProperty textProp;

		protected override void Init()
		{
			textProp = serializedObject.FindProperty("_text");
		}

		protected override void DrawGUI()
		{
			EditorGUILayout.PropertyField(textProp);

			var props = ThemesLibrary.Current.Colors.ToArray();
			var names = props.Select(x => x.Name).ToArray();
			var ids = props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(PropertyId.stringValue);
			if (index < 0)
				index = 0;

			var selectedIndex = EditorGUILayout.Popup("Color", index, names);
			if (selectedIndex != index)
			{
				PropertyId.stringValue = ids[selectedIndex];
				index = selectedIndex;
			}

			PointerOverriderEditor.DrawGUI(ids, names, index);
		}
	}
}