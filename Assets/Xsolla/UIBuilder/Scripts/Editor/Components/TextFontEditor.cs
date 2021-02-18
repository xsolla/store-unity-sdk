using System.Linq;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(TextFontDecorator))]
	public class TextFontEditor : ThemeDecoratorEditor
	{
		private SerializedProperty textProp;

		protected override void Init()
		{
			textProp = serializedObject.FindProperty("_text");
		}

		protected override void DrawGUI()
		{
			EditorGUILayout.PropertyField(textProp);

			var props = ThemesLibrary.Current.Fonts.ToArray();
			var names = props.Select(x => x.Name).ToArray();
			var ids = props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(PropertyId.stringValue);
			if (index < 0)
				index = 0;

			var selectedIndex = EditorGUILayout.Popup("Font", index, names);
			if (selectedIndex != index)
			{
				PropertyId.stringValue = ids[selectedIndex];
			}

			PointerOverriderEditor.DrawGUI(ids, names, index);
		}
	}
}