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
		public GameObject RedeemCouponPopupPrefab;
		public GameObject CouponRewardsPopupPrefab;
		public GameObject TutorialPopupPrefab;

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

		public ISuccessPopup CreateSuccess()
		{
			var popup = CreateDefaultPopup(SuccessPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<SuccessPopup>() : null;
		}

		public IErrorPopup CreateError()
		{
			var popup = CreateDefaultPopup(ErrorPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<ErrorPopup>() : null;
		}

		public IConfirmationPopup CreateConfirmation()
		{
			var popup = CreateDefaultPopup(ConfirmPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<ConfirmationPopup>() : null;
		}

		public IConfirmationCodePopup CreateCodeConfirmation()
		{
			var popup = CreateDefaultPopup(ConfirmCodePopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<ConfirmationCodePopup>() : null;
		}

		public IWaitingPopup CreateWaiting()
		{
			var popup = CreateDefaultPopup(WaitingPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<WaitingPopup>() : null;
		}

		public IBundlePreviewPopup CreateBundlePreview()
		{
			var popup = CreateDefaultPopup(BundlePreviewPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<BundlePreviewPopup>() : null;
		}

		public ISuccessPopup CreateSuccessPasswordReset()
		{
			var popup = CreateDefaultPopup(ResetPasswordPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<SuccessPopup>() : null;
		}

		public INicknamePopup CreateNickname()
		{
			var popup = CreateDefaultPopup(NicknamePopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<NicknamePopup>() : null;
		}

		public ICouponRedeemPopup CreateRedeemCoupon()
		{
			var popup = CreateDefaultPopup(RedeemCouponPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<CouponRedeemPopup>() : null;
		}
		
		public ICouponRewardsPopup CreateCouponRewards()
		{
			var popup = CreateDefaultPopup(CouponRewardsPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<CouponRewardsPopup>() : null;
		}

		public ITutorialPopup CreateTutorial()
		{
			var popup = CreateDefaultPopup(TutorialPopupPrefab, canvas);
			return (popup != null) ? popup.GetComponent<TutorialPopup>() : null;
		}
	}
}
