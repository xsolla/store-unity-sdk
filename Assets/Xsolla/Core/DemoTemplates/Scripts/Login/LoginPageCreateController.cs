using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class LoginPageCreateController : LoginPageController
{
#pragma warning disable 0649
	[SerializeField] private InputField UsernameInputField;
	[SerializeField] private InputField EmailInputField;
	[SerializeField] private InputField PasswordInputField;
	[SerializeField] private SimpleButton CreateButton;
#pragma warning restore 0649

	private bool IsCreateInProgress
	{
		get => base.IsInProgress;
		set
		{
			if (value == true)
			{
				base.OnStarted?.Invoke();
				Debug.Log("LoginPageCreateController: Create started");
			}
			else
				Debug.Log("LoginPageCreateController: Create ended");

			base.IsInProgress = value;
		}
	}

	private void Awake()
	{
		if (CreateButton != null)
			CreateButton.onClick += PrepareAndRunCreate;
	}

	private void PrepareAndRunCreate()
	{
		RunCreate(UsernameInputField.text, EmailInputField.text, PasswordInputField.text);
	}

	public void RunCreate(string username, string email, string password)
	{
		if (IsCreateInProgress)
			return;

		IsCreateInProgress = true;

		var isFieldsFilled = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
		var isEmailValid = ValidateEmail(email);

		if (isFieldsFilled && isEmailValid)
		{
			Action onSuccessfulCreate = () =>
			{
				Debug.Log("LoginPageCreateController: Create success");
				UserInfoContainer.LastEmail = email;
				base.OnSuccess?.Invoke();
			};

			Action<Error> onFailedCreate = error =>
			{
				Debug.LogError($"LoginPageCreateController: Create error: {error.ToString()}");
				base.OnError?.Invoke(error);
			};

			XsollaLogin.Instance.Registration(username, password, email, onSuccessfulCreate, onFailedCreate);
		}
		else if (!isEmailValid)
		{
			Debug.Log($"Invalid email: {email}");
			Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Invalid email");
			base.OnError?.Invoke(error);
		}
		else
		{
			Debug.LogError($"Fields are not filled. Username: '{username}' Password: '{password}'");
			Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: $"Not all fields are filled");
			base.OnError?.Invoke(error);
		}

		IsCreateInProgress = false;
	}

	private bool ValidateEmail(string email)
	{
		if (!string.IsNullOrEmpty(email))
		{
			var emailPattern = "^[a-zA-Z0-9-_.+]+[@][a-zA-Z0-9-_.]+[.][a-zA-Z]+$";
			var regex = new Regex(emailPattern);
			return regex.IsMatch(email);
		}
		else
		{
			return false;
		}
	}
}
