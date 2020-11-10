using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/RedeemCouponPopup")]
	public class RedeemCouponPopup : MonoBehaviour, IRedeemCouponPopup
	{
		[SerializeField] private InputField CodeInputField;
		[SerializeField] private Text ErrorMessage;
		[SerializeField] private SimpleTextButtonDisableable RedeemButton;
		[SerializeField] private SimpleTextButton CancelButton;

		private void Awake()
		{
			RedeemButton.Disable();

			if (RedeemButton != null)
			{
				RedeemButton.onClick = () => Destroy(gameObject, 0.001F);
			}

			if (CancelButton != null)
			{
				CancelButton.onClick = () => Destroy(gameObject, 0.001F);
			}

			if (CodeInputField != null)
			{
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
		}

		public IRedeemCouponPopup SetRedeemCallback(Action<string> buttonPressed)
		{
			RedeemButton.onClick = () =>
			{
				ErrorMessage.gameObject.SetActive(false);
				buttonPressed?.Invoke(CodeInputField?.text);
			};
			return this;
		}

		public IRedeemCouponPopup SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public IRedeemCouponPopup ShowError()
		{
			ErrorMessage.gameObject.SetActive(true);
			return this;
		}
	}
}