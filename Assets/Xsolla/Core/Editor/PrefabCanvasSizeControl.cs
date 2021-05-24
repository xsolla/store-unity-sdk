#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Xsolla.Demo
{
	/// <summary>
	/// Watches for state changes of prefab stage. 
	/// Adds special component to the root game object when prefab gets open,
	/// destroys it when prefab is being closed.
	/// </summary>
	[InitializeOnLoad]
	static class PrefabStageCanvasSize
	{
		static PrefabStageCanvasSize()
		{
			if (EditorApplication.isPlaying)
				return;

			PrefabStage.prefabStageOpened += stage =>
			{
				// get RectTranform of prefab environment
				var envRT = stage.prefabContentsRoot.transform.parent as RectTransform;
				if (envRT == null)
					return; // non UI prefab

				// although add/destroy of components is not prohibited
				// for environment game object, but it's not editable in inspector,
				// so we add component to root game object of prefab
				var resizer = stage.prefabContentsRoot.AddComponent<Resizer>();

				// to be sure it won't be saved in anyways
				resizer.hideFlags = HideFlags.DontSave;

				resizer.Init(envRT, envRT.GetComponent<Canvas>());
			};

			PrefabStage.prefabStageClosing += stage =>
			{
				// destroy component to avoid memory leaks when using DontSave flags
				var resizer = stage.prefabContentsRoot.GetComponent<Resizer>();
				UnityEngine.Object.DestroyImmediate(resizer);
			};
		}

		/// <summary>
		/// Changes environment canvas size according to chosen mode in the inspector.
		/// This class made nested to not show it up in [Add Component] drop down list.
		/// </summary>
		[ExecuteAlways]
		public class Resizer : MonoBehaviour
		{
			// change default values if needed
			public Vector2 referenceSize = new Vector2(1920, 1080);

			// components of environment game object
			private RectTransform _envRect;
			private Canvas _envCanvas;

			public void Init(RectTransform envRect, Canvas envCanvas)
			{
				_envRect = envRect;
				_envCanvas = envCanvas;

				SetMode();
			}

			public void SetMode()
			{
				if (_envRect == null || _envCanvas == null)
					return;
				_envCanvas.renderMode = RenderMode.WorldSpace;
				_envRect.sizeDelta = referenceSize;
			}
		}

		/// <summary>
		/// The purpose of this custom editor is to avoid dirtying of the prefab
		/// caused by modifications of Resizer properties in the inspector.
		/// </summary>
		[CustomEditor(typeof(Resizer))]
		public class PrefabStageCanvasSizeEditor : Editor
		{
			private SerializedProperty _referenceSize;

			public void OnEnable()
			{
				_referenceSize = serializedObject.FindProperty("referenceSize");
			}

			public override void OnInspectorGUI()
			{
				EditorGUILayout.HelpBox(
					"This component drives environment canvas size and unties it " +
					"from Game View size. It is added/removed automatically when " +
					"you open/close prefab and makes no changes to it.",
					MessageType.Info);

				var resizer = (Resizer) target;

				serializedObject.Update();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_referenceSize);
				if (EditorGUI.EndChangeCheck())
				{
					// change values in the component instance directly
					// without applying modified properties to serializedObject
					resizer.referenceSize = _referenceSize.vector2Value;
					resizer.SetMode();
				}
			}
		}
	}
}
#endif