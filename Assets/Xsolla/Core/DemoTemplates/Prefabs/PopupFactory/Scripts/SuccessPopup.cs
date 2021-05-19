using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/SuccessPopup")]
	public class SuccessPopup : MonoBehaviour, ISuccessPopup
	{
		[SerializeField] private Text title;
		[SerializeField] private Text message;
		[SerializeField] private SimpleTextButton button;
		[SerializeField] private Text buttonText;

		private void Awake()
		{
			if (button != null)
			{
				button.onClick = () => Destroy(gameObject, 0.001F);
			}
		}

		ISuccessPopup ISuccessPopup.SetButtonText(string text)
		{
			if (!string.IsNullOrEmpty(text))
				buttonText.text = text;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetCallback(Action buttonPressed)
		{
			button.onClick = () =>
			{
				if (buttonPressed != null)
					buttonPressed.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		ISuccessPopup ISuccessPopup.SetMessage(string messageText)
		{
			if (!string.IsNullOrEmpty(messageText))
				message.text = messageText;
			return this;
		}

		ISuccessPopup ISuccessPopup.SetTitle(string titleText)
		{
			if (!string.IsNullOrEmpty(titleText))
				title.text = titleText;
			return this;
		}
	}
}
