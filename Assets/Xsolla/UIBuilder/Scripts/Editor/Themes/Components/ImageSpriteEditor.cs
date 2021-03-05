using System.Linq;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(ImageSpriteDecorator))]
	public class ImageSpriteEditor : ThemeDecoratorEditor
	{
		private SerializedProperty imageProp;

		protected override void Init()
		{
			imageProp = serializedObject.FindProperty("_image");
		}

		protected override void DrawGUI()
		{
			EditorGUILayout.PropertyField(imageProp);

			var props = ThemesLibrary.Current.Sprites.ToArray();
			var names = props.Select(x => x.Name).ToArray();
			var ids = props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(PropertyId.stringValue);
			if (index < 0)
				index = 0;

			var selectedIndex = EditorGUILayout.Popup("Sprite", index, names);
			if (selectedIndex != index)
			{
				PropertyId.stringValue = ids[selectedIndex];
				index = selectedIndex;
			}

			PointerOverriderEditor.DrawGUI(ids, names, index);
		}
	}
}