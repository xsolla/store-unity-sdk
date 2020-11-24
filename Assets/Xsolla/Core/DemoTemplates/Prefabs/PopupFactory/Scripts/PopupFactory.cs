using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.Core.Popup
{
	internal static class GameObjectPopupExtensions
	{
		public static GameObject CreateObjectFor(this GameObject prefab, GameObject parent)
		{
			GameObject result;
			if (parent != null)
			{
				result = Object.Instantiate(prefab, parent.transform);
			}
			else
			{
				result = Object.Instantiate(prefab);
			}

			result.name = prefab.name;
			return result;
		}
	}

	[AddComponentMenu("Scripts/Xsolla.Core/Popup/PopupFactory")]
	public class PopupFactory : MonoSingleton<PopupFactory>
	{
		public GameObject SuccessPopupPrefab;
		public GameObject ErrorPopupPrefab;
		public GameObject ConfirmPopupPrefab;
		public GameObject ConfirmCodePopupPrefab;
		public GameObject WaitingPopupPrefab;
		public GameObject BundlePreviewPopupPrefab;
		public GameObject ResetPasswordPopupPrefab;
		public GameObject NicknamePopupPrefab;

		private GameObject canvas;

		public override void Init()
		{
			base.Init();

			Canvas canvasComponent = GameObject.FindObjectOfType<Canvas>();
			if (canvasComponent != null)
			{
				canvas = canvasComponent.gameObject;
			}
			else
			{
				Debug.LogError("You try use 2D popup component, but Canvas object is missing!");
			}
		}

		private GameObject CreateDefaultPopup(GameObject prefab, GameObject parent)
		{
			if (prefab == null)
			{
				Debug.LogError(
					"You try create object, but prefab for it is null. " + Environment.NewLine +
					"Check `PopupFactory` script for missing prefabs.");
				return null;
			}

			return prefab.CreateObjectFor(parent);
		}

		public ISuccessPopup CreateSuccess() =>
			CreateDefaultPopup(SuccessPopupPrefab, canvas)?.GetComponent<SuccessPopup>();

		public IErrorPopup CreateError() => CreateDefaultPopup(ErrorPopupPrefab, canvas)?.GetComponent<ErrorPopup>();

		public IConfirmationPopup CreateConfirmation() =>
			CreateDefaultPopup(ConfirmPopupPrefab, canvas)?.GetComponent<ConfirmationPopup>();

		public IConfirmationCodePopup CreateCodeConfirmation() => CreateDefaultPopup(ConfirmCodePopupPrefab, canvas)
			?.GetComponent<ConfirmationCodePopup>();

		public IWaitingPopup CreateWaiting() =>
			CreateDefaultPopup(WaitingPopupPrefab, canvas)?.GetComponent<WaitingPopup>();

		public IBundlePreviewPopup CreateBundlePreview() =>
			CreateDefaultPopup(BundlePreviewPopupPrefab, canvas)?.GetComponent<BundlePreviewPopup>();

		public ISuccessPopup CreateSuccessPasswordReset() =>
			CreateDefaultPopup(ResetPasswordPopupPrefab, canvas)?.GetComponent<SuccessPopup>();

		public INicknamePopup CreateNickname() =>
			CreateDefaultPopup(NicknamePopupPrefab, canvas)?.GetComponent<NicknamePopup>();
	}
}