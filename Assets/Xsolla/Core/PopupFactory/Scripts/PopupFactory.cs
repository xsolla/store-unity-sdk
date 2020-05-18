using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.Core.Popup
{
	internal static class GameObjectPopupExtensions
	{
		public static GameObject AddBackground(this GameObject popup, GameObject backgroundPrefab)
		{
			backgroundPrefab?.CreateObjectFor(popup).transform.SetSiblingIndex(0);
			return popup;
		}

		public static GameObject CreateObjectFor(this GameObject prefab, GameObject parent)
		{
			GameObject result;
			if (parent != null) {
				result = Object.Instantiate(prefab, parent.transform);
			} else {
				Debug.LogError("Parent for Popup is null. Can not find Canvas. Something went wrong.");
				result = Object.Instantiate(prefab);
			}
			result.name = prefab.name;
			result.transform.localPosition = Vector3.zero;
			return result;
		}
	}

	[AddComponentMenu("Scripts/Xsolla.Core/Popup/PopupFactory")]
	public class PopupFactory : MonoSingleton<PopupFactory>
	{
		public GameObject BackgroundPrefab;
		public GameObject SuccessPopupPrefab;
		public GameObject ErrorPopupPrefab;
		public GameObject ConfirmPopupPrefab;
		public GameObject ConfirmCodePopupPrefab;

		private static GameObject Canvas {
			get
			{
				Canvas canvasComponent = FindObjectOfType<Canvas>();
				if (canvasComponent != null)
					return canvasComponent.gameObject;
				Debug.LogError("You try use 2D popup component, but Canvas object is missing!");
				return null;
			}
		}

		private GameObject CreateDefaultPopup(GameObject prefab, GameObject parent)
		{
			if (prefab == null) {
				Debug.LogError(
					"You try create object, but prefab for it is null. " + Environment.NewLine +
					"Check `PopupFactory` script for missing prefabs.");
				return null;
			}
			return prefab.CreateObjectFor(parent).AddBackground(BackgroundPrefab);
		}

		public ISuccessPopup CreateSuccess() => CreateDefaultPopup(SuccessPopupPrefab, Canvas)?.
			GetComponent<SuccessPopup>();

		public IErrorPopup CreateError() => CreateDefaultPopup(ErrorPopupPrefab, Canvas)?.
			GetComponent<ErrorPopup>();

		public IConfirmationPopup CreateConfirmation() => CreateDefaultPopup(ConfirmPopupPrefab, Canvas)?.
			GetComponent<ConfirmationPopup>();

		public IConfirmationCodePopup CreateCodeConfirmation() => CreateDefaultPopup(ConfirmCodePopupPrefab, Canvas)?.
			GetComponent<ConfirmationCodePopup>();
	}
}
