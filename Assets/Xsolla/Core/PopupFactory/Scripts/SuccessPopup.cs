using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/SuccessPopup")]
	public class SuccessPopup : MonoBehaviour, ISuccessPopup
	{
		[SerializeField]
		private Text Title;
		[SerializeField]
		private Text Message;
		[SerializeField]
		private SimpleTextButton Button;
		[SerializeField]
		private Text ButtonText;

		void Awake()
		{
			if(Button != null) {
				Button.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		ISuccessPopup ISuccessPopup.SetButtonText(string buttonText)
		{
			if(!string.IsNullOrEmpty(buttonText))
				ButtonText.text = buttonText;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetCallback(Action buttonPressed)
		{			
			Button.onClick = () => {
				buttonPressed?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		ISuccessPopup ISuccessPopup.SetMessage(string messageText)
		{
			if(!string.IsNullOrEmpty(messageText))
				Message.text = messageText;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetTitle(string titleText)
		{
			if (!string.IsNullOrEmpty(titleText))
				Title.text = titleText;
			return this;
		}
	}
}
