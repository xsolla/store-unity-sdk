using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/ConfirmationCodePopup")]
	public class ConfirmationCodePopup : MonoBehaviour, IConfirmationCodePopup
	{
		[SerializeField] private Text Title;
		[SerializeField] private Text InputCodeText;
		[SerializeField] private SimpleTextButton ConfirmButton;
		[SerializeField] private Text ConfirmButtonText;
		[SerializeField] private SimpleTextButton CancelButton;
		[SerializeField] private Text CancelButtonText;

		private void Awake()
		{
			if (ConfirmButton != null)
			{
				ConfirmButton.onClick = () => Destroy(gameObject, 0.001F);
			}

			if (CancelButton != null)
			{
				CancelButton.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		IConfirmationCodePopup IConfirmationCodePopup.SetTitle(string titleText)
		{
			Title.text = titleText;
			return this;
		}

		IConfirmationCodePopup IConfirmationCodePopup.SetConfirmCallback(Action<string> buttonPressed)
		{
			ConfirmButton.onClick = () =>
			{
				buttonPressed?.Invoke(InputCodeText?.text);
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationCodePopup IConfirmationCodePopup.SetConfirmButtonText(string buttonText)
		{
			ConfirmButtonText.text = buttonText;
			return this;
		}

		IConfirmationCodePopup IConfirmationCodePopup.SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () =>
			{
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationCodePopup IConfirmationCodePopup.SetCancelButtonText(string buttonText)
		{
			CancelButtonText.text = buttonText;
			return this;
		}
	}
}