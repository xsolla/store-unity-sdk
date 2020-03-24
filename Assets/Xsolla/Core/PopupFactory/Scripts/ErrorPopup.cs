using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/ErrorPopup")]
	public class ErrorPopup : MonoBehaviour, IErrorPopup
	{
		[SerializeField]
		private Text Title;
		[SerializeField]
		private Text Message;
		[SerializeField]
		private SimpleTextButton Button;
		[SerializeField]
		private Text ButtonText;

		IErrorPopup IErrorPopup.SetButtonText(string buttonText)
		{
			ButtonText.text = buttonText;
			return this;
		}

		IErrorPopup IErrorPopup.SetCallback(Action buttonPressed)
		{
			Button.onClick = () => {
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		IErrorPopup IErrorPopup.SetMessage(string messageText)
		{
			Message.text = messageText;
			return this;
		}

		IErrorPopup IErrorPopup.SetTitle(string titleText)
		{
			Title.text = titleText;
			return this;
		}
	}
}
