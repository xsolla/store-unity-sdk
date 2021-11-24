using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
    public class PasswordlessWidget : MonoBehaviour, ICodeRequester
	{
		[SerializeField] private GameObject ChoiceState;
		[SerializeField] private GameObject PhoneState;
		[SerializeField] private GameObject EmailState;
		[SerializeField] private GameObject CodeState;
		[SerializeField] private SimpleButton BackButton;
		[Space]
		[SerializeField] private SimpleButton PhoneChoiceButton;
		[SerializeField] private SimpleButton EmailChoiceButton;
		[Space]
		[SerializeField] private InputField PhoneInputField;
		[SerializeField] private UserProfileEntryPhoneValueConverter PhoneConverter;
		[SerializeField] private SimpleTextButtonDisableable PhoneSendButton;
		[Space]
		[SerializeField] private InputField EmailInputField;
		[SerializeField] private SimpleTextButtonDisableable EmailSendButton;
		[Space]
		[SerializeField] private InputField CodeInputField;
		[SerializeField] private SimpleTextButtonDisableable CodeSendButton;
		[SerializeField] private SimpleButton ResendButton;
		[SerializeField] private CountdownTimer Timer;

		private AuthType _currentAuthType = AuthType.None;

		public event Action<string> OnPhoneAccessRequest;
		public event Action<string> OnEmailAccessRequest;

		private void Awake()
		{
			SetState(ChoiceState);
			BackButton.onClick += () => SetState(ChoiceState);

			PhoneChoiceButton.onClick += OnPhoneChoice;
			PhoneInputField.onValueChanged.AddListener(OnPhoneInput);
			PhoneSendButton.onClick += OnPhoneRequest;

			EmailChoiceButton.onClick += OnEmailChoice;
			EmailInputField.onValueChanged.AddListener(OnEmailInput);
			EmailSendButton.onClick += OnEmailRequest;

			CodeInputField.onValueChanged.AddListener(OnCodeInput);
			ResendButton.onClick += OnResendButton;
			Timer.TimeIsUp += OnTimerEnd;
		}

		public void RequestCode(Action<string> onCode)
		{
			SetState(CodeState);
			CodeInputField.text = string.Empty;
			CodeSendButton.Disable();
			CodeSendButton.onClick = () =>
			{
				if (onCode != null)
					onCode.Invoke(CodeInputField.text);
			};
			Timer.ResetTimer();
		}

		public void RaiseOnError(Error error)
		{
			if (error.ErrorType == ErrorType.InvalidAuthorizationCode)
				StoreDemoPopup.ShowError(error);
			else
				StoreDemoPopup.ShowError(error, CloseWidget);
		}

		private void OnPhoneChoice()
		{
			SetState(PhoneState);
			OnPhoneInput(PhoneInputField.text);
		}

		private void OnPhoneInput(string text)
		{
			var phone = PhoneConverter.ConvertBack(text);

			if (IsPhoneValid(phone))
				PhoneSendButton.Enable();
			else
				PhoneSendButton.Disable();
		}

		private bool IsPhoneValid(string phone)
		{
			if (string.IsNullOrEmpty(phone))
				return false;

			var regex = new Regex("^\\+(\\d){5,25}string.Format(");
			return regex.IsMatch(phone);
		}

		private void OnPhoneRequest()
		{
			var phone = PhoneConverter.ConvertBack(PhoneInputField.text);
			Debug.Log(string.Format("PasswordlessWidget: phone is '{0}'", phone));
			_currentAuthType = AuthType.Phone;
			if (OnPhoneAccessRequest != null)
				OnPhoneAccessRequest.Invoke(phone);
		}

		private void OnEmailChoice()
		{
			SetState(EmailState);
			OnEmailInput(EmailInputField.text);
		}

		private void OnEmailInput(string text)
		{
			if (text.Length > 1 && text.Length < 255)
				EmailSendButton.Enable();
			else
				EmailSendButton.Disable();
		}

		private void OnEmailRequest()
		{
			var email = EmailInputField.text;
			Debug.Log(string.Format("PasswordlessWidget: email is '{0}'", email));
			_currentAuthType = AuthType.Email;
			if (OnEmailAccessRequest != null)
				OnEmailAccessRequest.Invoke(email);
		}

		private void OnCodeInput(string code)
		{
			if (code.Length >= 1)
				CodeSendButton.Enable();
			else
				CodeSendButton.Disable();
		}

		private void OnResendButton()
		{
			switch (_currentAuthType)
			{
				case AuthType.Phone:
					OnPhoneRequest();
					break;
				case AuthType.Email:
					//TODO
					break;
				default:
					Debug.LogError(string.Format("PasswordlessWidget.OnResendButton: Unexpected auth type '{0}'", _currentAuthType));
					break;
			}

			Timer.ResetTimer();
		}

		private void OnTimerEnd()
		{
			StoreDemoPopup.ShowWarning(
				error: new Error(errorMessage: "Code expired"),
				buttonCallback: CloseWidget);
		}

		private void CloseWidget()
		{
			if (BackButton && BackButton.onClick != null)
				BackButton.onClick.Invoke();
		}

		private void SetState(GameObject targetState)
		{
			Timer.StopTimer();

			var allStates = new GameObject[] { ChoiceState, PhoneState, EmailState, CodeState };
			foreach (var state in allStates)
				SetActive(state, state.Equals(targetState));
		}

		private void SetActive(GameObject stateObject, bool targetState)
		{
			if (stateObject.activeSelf != targetState)
				stateObject.SetActive(targetState);
		}

		private enum AuthType : byte
		{
			None, Phone, Email
		}
	}
}
