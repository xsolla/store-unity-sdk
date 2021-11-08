using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
    public class PasswordlessWidget : MonoBehaviour
    {
		[SerializeField] private GameObject ChoiceState = default;
		[SerializeField] private GameObject PhoneState = default;
		[SerializeField] private GameObject EmailState = default;
		[SerializeField] private GameObject ConfirmationState = default;
		[SerializeField] private SimpleButton BackButton = default;
		[Space]
		[SerializeField] private SimpleButton PhoneChoiceButton = default;
		[SerializeField] private SimpleButton EmailChoiceButton = default;
		[Space]
		[SerializeField] private SimpleButton PhoneSendButton = default;
		[Space]
		[SerializeField] private SimpleButton EmailSendButton = default;

		private void Awake()
		{
			SetState(ChoiceState);

			BackButton.onClick += () => SetState(ChoiceState);

			PhoneChoiceButton.onClick += () => SetState(PhoneState);
			EmailChoiceButton.onClick += () => SetState(EmailState);
			PhoneSendButton.onClick += () => SetState(ConfirmationState);
			EmailSendButton.onClick += () => SetState(ConfirmationState);
		}

		private void SetState(GameObject targetState)
		{
			var allStates = new GameObject[] { ChoiceState, PhoneState, EmailState, ConfirmationState };
			foreach (var state in allStates)
				SetActive(state, state.Equals(targetState));
		}

		private void SetActive(GameObject stateObject, bool targetState)
		{
			if (stateObject.activeSelf != targetState)
				stateObject.SetActive(targetState);
		}
	}
}
