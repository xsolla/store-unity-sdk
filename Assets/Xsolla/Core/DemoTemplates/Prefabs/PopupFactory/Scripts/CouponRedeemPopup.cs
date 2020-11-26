using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/RedeemCouponPopup")]
	public class CouponRedeemPopup : MonoBehaviour, ICouponRedeemPopup
	{
		[SerializeField] private InputField CodeInputField;
		[SerializeField] private Text ErrorMessage;
		[SerializeField] private SimpleTextButtonDisableable RedeemButton;
		[SerializeField] private SimpleTextButton CancelButton;

		private void Awake()
		{
			RedeemButton.Disable();
			RedeemButton.onClick = () => Destroy(gameObject, 0.001F);

			CancelButton.onClick = () => Destroy(gameObject, 0.001F);

			CodeInputField.onValueChanged.AddListener(newValue =>
			{
				if (string.IsNullOrEmpty(newValue))
				{
					RedeemButton?.Disable();
				}
				else
				{
					RedeemButton?.Enable();
				}
			});
		}

		public ICouponRedeemPopup SetRedeemCallback(Action<string> buttonPressed)
		{
			RedeemButton.onClick = () =>
			{
				ErrorMessage.gameObject.SetActive(false);
				buttonPressed?.Invoke(CodeInputField?.text);
			};
			return this;
		}

		public ICouponRedeemPopup SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public ICouponRedeemPopup ShowError()
		{
			ErrorMessage.gameObject.SetActive(true);
			return this;
		}

		public void Close()
		{
			Destroy(gameObject, 0.001F);
		}
	}
}