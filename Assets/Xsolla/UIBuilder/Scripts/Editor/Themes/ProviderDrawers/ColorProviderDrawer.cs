using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomPropertyDrawer(typeof(ColorProvider))]
	public class ColorProviderDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var idProp = property.FindPropertyRelative("_id");

			var props = ThemesLibrary.Current.Colors.ToArray();
			var names = props.Select(x => x.Name).ToArray();
			var ids = props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(idProp.stringValue);
			if (index < 0)
				index = 0;

			var selectedIndex = EditorGUI.Popup(position, property.name, index, names);
			if (selectedIndex != index)
			{
				idProp.stringValue = ids[selectedIndex];
			}

			EditorGUI.EndProperty();
		}
	}
}