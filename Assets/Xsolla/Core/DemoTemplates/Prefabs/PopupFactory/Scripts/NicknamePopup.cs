using System;
using UnityEngine;

namespace Xsolla.Core.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/NicknamePopup")]
	public class NicknamePopup : MonoBehaviour, INicknamePopup
	{
#pragma warning disable 0649
		[SerializeField] SimpleButton ContinueButton;
		[SerializeField] SimpleButton CancelButton;
		[SerializeField] UserProfileEntryEditor Editor;
#pragma warning restore 0649

		private string UserInput { get; set; }

		private void Awake()
		{
			Editor.UserProfileEntryEdited += newValue => UserInput = newValue;
		}

		public INicknamePopup SetCallback(Action<string> nicknameCallback)
		{
			ContinueButton.onClick = () =>
			{
				nicknameCallback?.Invoke(UserInput);
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public INicknamePopup SetCancelCallback(Action cancelCallback)
		{
			CancelButton.onClick = () =>
			{
				cancelCallback?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}
	}
}