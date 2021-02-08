using UnityEditor;

namespace Xsolla.UIBuilder
{
	public abstract class ThemeDecoratorEditor : Editor
	{
		protected SerializedProperty propId;

		private static bool IsDebugMode => false;

		protected void OnEnable()
		{
			propId = serializedObject.FindProperty("_propertyId");
			FindProperties();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (IsDebugMode)
			{
				EditorGUILayout.PropertyField(propId);
			}

			DrawProperties();
			serializedObject.ApplyModifiedProperties();
		}

		protected abstract void FindProperties();

		protected abstract void DrawProperties();
	}
}