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
		
		if (XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID || 
		    XsollaSettings.LoginId == Constants.DEFAULT_PLATFORM_LOGIN_ID)
		{
			if (XsollaSettings.StoreProjectId != Constants.DEFAULT_PROJECT_ID)
			{
				error.errorMessage = $"You change [XsollaSettings->ProjectID] to '{XsollaSettings.StoreProjectId}', but not change LoginID. Change LoginID from '{XsollaSettings.LoginId}' to correct value.";
				StoreDemoPopup.ShowError(error);
			}
		}
		else
		{
			if (XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID)
			{
				error.errorMessage = $"You change [XsollaSettings->LoginID] to '{XsollaSettings.LoginId}', but not change ProjectID. Change ProjectID from '{XsollaSettings.StoreProjectId}' to correct value.";
				StoreDemoPopup.ShowError(error);
			}
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
