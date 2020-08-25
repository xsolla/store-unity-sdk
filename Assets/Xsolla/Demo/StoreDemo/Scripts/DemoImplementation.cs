using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	private void Start()
	{
		DemoController.Instance.StateChangingEvent += (state, newState) =>
		{
			if (newState == MenuState.Main)
			{
				XsollaLogin.Instance.Token = XsollaStore.Instance.Token = GetUserToken();
			}
		};
		ValidateXsollaSettings();
	}

	private void ValidateXsollaSettings()
	{
		var error = Error.UnknownError;
		
		if (string.IsNullOrEmpty(XsollaSettings.LoginId))
		{
			error.errorMessage = "Xsolla Login settings were not completed. Please copy the Login project ID from your Publisher Account and add it to your project settings";
			StoreDemoPopup.ShowError(error);
			return;
		}

		if (string.IsNullOrEmpty(XsollaSettings.StoreProjectId))
		{
			error.errorMessage = "Xsolla Login settings were not completed. Please copy the Store Project ID from your Publisher Account and add it to your project settings";
			StoreDemoPopup.ShowError(error);
			return;
		}

		var isDefaultLoginID = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID || XsollaSettings.LoginId == Constants.DEFAULT_PLATFORM_LOGIN_ID;
		var isDefaultProjectID = XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;

		if (isDefaultLoginID && !isDefaultProjectID)
		{
			error.errorMessage = $"You changed [XsollaSettings->ProjectID] to '{XsollaSettings.StoreProjectId}', but did not change LoginID. Change LoginID from '{XsollaSettings.LoginId}' to correct value.";
			StoreDemoPopup.ShowError(error);
		}
		else if (!isDefaultLoginID && isDefaultProjectID)
		{
			error.errorMessage = $"You changed [XsollaSettings->LoginID] to '{XsollaSettings.LoginId}', but did not change ProjectID. Change ProjectID from '{XsollaSettings.StoreProjectId}' to correct value.";
			StoreDemoPopup.ShowError(error);
		}
	}

	public Token GetUserToken()
	{
		if (Token == null || Token.IsNullOrEmpty())
		{
			return DemoController.Instance.GetImplementation().GetDemoUserToken();
		}
		return Token;
	}

	private Action<Error> WrapErrorCallback(Action<Error> onError)
	{
		return error =>
		{
			StoreDemoPopup.ShowError(error);
			onError?.Invoke(error);
		};
	}
}
