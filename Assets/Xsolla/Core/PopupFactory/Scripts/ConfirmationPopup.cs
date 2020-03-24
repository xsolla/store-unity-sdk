using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/ConfirmationPopup")]
	public class ConfirmationPopup : MonoBehaviour, IConfirmationPopup
	{
		[SerializeField]
		private Text Title;
		[SerializeField]
		private Text Message;
		[SerializeField]
		private SimpleTextButton ConfirmButton;
		[SerializeField]
		private Text ConfirmButtonText;
		[SerializeField]
		private SimpleTextButton CancelButton;
		[SerializeField]
		private Text CancelButtonText;

		IConfirmationPopup IConfirmationPopup.SetMessage(string messageText)
		{
			Message.text = messageText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetTitle(string titleText)
		{
			Title.text = titleText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetConfirmCallback(Action buttonPressed)
		{
			ConfirmButton.onClick = () => {
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetConfirmButtonText(string buttonText)
		{
			ConfirmButtonText.text = buttonText;
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetCancelCallback(Action buttonPressed)
		{
			CancelButton.onClick = () => {
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IConfirmationPopup IConfirmationPopup.SetCancelButtonText(string buttonText)
		{
			CancelButtonText.text = buttonText;
			return this;
		}
	}
}
