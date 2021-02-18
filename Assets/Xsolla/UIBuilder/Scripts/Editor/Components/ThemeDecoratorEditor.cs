using UnityEditor;

namespace Xsolla.UIBuilder
{
	public abstract class ThemeDecoratorEditor : Editor
	{
		protected SerializedProperty PropertyId { get; private set; }

		protected PointerOverriderEditor PointerOverriderEditor { get; private set; }

		protected void OnEnable()
		{
			PropertyId = serializedObject.FindProperty("_propertyId");
			PointerOverriderEditor = new PointerOverriderEditor(serializedObject);
			Init();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawGUI();
			serializedObject.ApplyModifiedProperties();
		}

		protected abstract void Init();

		protected abstract void DrawGUI();
	}
}