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
