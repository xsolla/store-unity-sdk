using System;
using UnityEngine;
using Xsolla.Demo;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/NicknamePopup")]
	public class NicknamePopup : MonoBehaviour, INicknamePopup
	{
		[SerializeField] SimpleButton ContinueButton;
		[SerializeField] SimpleButton CancelButton;
		[SerializeField] UserProfileEntryEditor Editor;

		private string UserInput { get; set; }

		private void Awake()
		{
			Editor.UserProfileEntryEdited += newValue => UserInput = newValue;
		}

		public INicknamePopup SetCallback(Action<string> nicknameCallback)
		{
			ContinueButton.onClick = () =>
			{
				if (nicknameCallback != null)
					nicknameCallback.Invoke(UserInput);
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public INicknamePopup SetCancelCallback(Action cancelCallback)
		{
			CancelButton.onClick = () =>
			{
				if (cancelCallback != null)
					cancelCallback.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}
	}
}
