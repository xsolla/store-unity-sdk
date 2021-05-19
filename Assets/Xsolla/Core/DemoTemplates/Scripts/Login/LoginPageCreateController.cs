using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class LoginPageCreateController : LoginPageController
{
#pragma warning disable 0649
	[SerializeField] private InputField UsernameInputField;
	[SerializeField] private InputField EmailInputField;
	[SerializeField] private InputField PasswordInputField;
	[SerializeField] private SimpleButton CreateButton;
#pragma warning restore 0649

	public static string LastUsername { get; private set; }
	public static string LastEmail { get; private set; }

	public static void DropLastCredentials()
	{
		LastUsername = null;
		LastEmail = null;
	}

	private bool IsCreateInProgress
	{
		get { return base.IsInProgress; }
		set
		{
			if (value == true)
			{
				if (base.OnStarted != null)
					base.OnStarted.Invoke();

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

	private void Start()
	{
		if (!string.IsNullOrEmpty(LastUsername))
			UsernameInputField.text = LastUsername;

		if (!string.IsNullOrEmpty(LastEmail))
			EmailInputField.text = LastEmail;
	}

	private void PrepareAndRunCreate()
	{
		RunCreate(UsernameInputField.text, EmailInputField.text, PasswordInputField.text);
	}

	public void RunCreate(string username, string email, string password)
	{
		if (IsCreateInProgress)
			return;

		LastEmail = email;
		LastUsername = username;

		IsCreateInProgress = true;

		var isFieldsFilled = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
		var isEmailValid = ValidateEmail(email);

		if (isFieldsFilled && isEmailValid)
		{
			Action onSuccessfulCreate = () =>
			{
				Debug.Log("LoginPageCreateController: Create success");
				if (base.OnSuccess != null)
					base.OnSuccess.Invoke();
			};

			Action<Error> onFailedCreate = error =>
			{
				Debug.LogError(string.Format("LoginPageCreateController: Create error: {0}", error.ToString()));
				if (base.OnError != null)
					base.OnError.Invoke(error);
			};

			DemoController.Instance.GetImplementation().Registration(username, password, email, onSuccessfulCreate, onFailedCreate);
		}
		else if (!isEmailValid)
		{
			Debug.Log(string.Format("Invalid email: {0}", email));
			Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Invalid email");
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}
		else
		{
			Debug.LogError(string.Format("Fields are not filled. Username: '{0}' Password: '{1}'", username, password));
			Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Not all fields are filled");
			if (base.OnError != null)
				base.OnError.Invoke(error);
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
