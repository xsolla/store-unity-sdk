using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.UIBuilder
{
	public static class WidgetsHandler
	{
		public static void Handle(WidgetContainer container)
		{
			var assetPath = GetAssetPath(container);
			GameObject prefab = null;

			if (!string.IsNullOrEmpty(assetPath))
			{
				prefab = PrefabUtility.LoadPrefabContents(assetPath);
				container = prefab.GetComponent<WidgetContainer>();
			}

			var widget = WidgetsLibrary.GetWidgetProperty(container.PropertyId).Prefab;

			ClearCurrentWidget(container);
			SetCurrentWidget(container, widget);
			AlignWidgetByContainer(container.Container, container.Current);

			if (!string.IsNullOrEmpty(assetPath))
			{
				PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
				PrefabUtility.UnloadPrefabContents(prefab);
			}
		}

		private static string GetAssetPath(WidgetContainer container)
		{
			if (Application.isEditor)
			{
				return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(container);
			}

			return null;
		}

		private static void ClearCurrentWidget(WidgetContainer container)
		{
			if (container.Current)
			{
				DestroyObject(container.Current.gameObject);
			}

			container.Current = null;
		}

		private static void SetCurrentWidget(WidgetContainer container, GameObject target)
		{
			if (container && target)
			{
				container.Current = CreateAsChild(container.Container, target).transform;
			}
		}

		private static void DestroyObject(GameObject gameObject)
		{
			if (Application.isEditor)
			{
				Object.DestroyImmediate(gameObject);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}

		private static GameObject CreateAsChild(Transform parent, GameObject prefab)
		{
			GameObject gameObject;
			if (Application.isEditor)
			{
				gameObject = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
			}
			else
			{
				gameObject = Object.Instantiate(prefab, parent, false);
			}

			if (!gameObject || gameObject == null)
				throw new Exception();

			gameObject.name = prefab.name;
			gameObject.transform.SetParent(parent, false);

			return gameObject;
		}

		private static void AlignWidgetByContainer(Transform parent, Transform child)
		{
			if (!parent || !child)
				return;

			var rectTransform = child.transform as RectTransform;
			if (!rectTransform || rectTransform == null)
				return;

			rectTransform.SetParent(parent, false);

			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;

			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			rectTransform.sizeDelta = Vector2.zero;
		}
	}
}