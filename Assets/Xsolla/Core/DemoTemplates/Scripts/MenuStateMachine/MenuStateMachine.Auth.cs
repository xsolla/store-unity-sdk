using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

public partial class MenuStateMachine : MonoBehaviour, IMenuStateMachine
{
	void CheckAuthorizationState(MenuState state, GameObject go)
	{
		if(go == null) return;
		
		var pageController = go.GetComponent<LoginPageController>();
		var buttonsProvider = go.GetComponent<LoginPageCommonButtonsProvider>();
		
		if(pageController == null && buttonsProvider == null) return;

		if (buttonsProvider != null)
		{
			if (buttonsProvider.LogInButton != null)
			{
				if(state == MenuState.Authorization)
					buttonsProvider.LogInButton.onClick += () => SetState(MenuState.Registration);
				else
					buttonsProvider.LogInButton.onClick += () => SetState(MenuState.Authorization);
			}
			if (buttonsProvider.DemoUserButton != null)
				buttonsProvider.DemoUserButton.onClick += () => SetState(MenuState.Main);
		}

		switch (state)
		{
			case MenuState.Authorization:
			{
				if (pageController != null)
				{
					pageController.OnSuccess = () => SetState(MenuState.Main);
					pageController.OnError = _ => SetState(MenuState.AuthorizationFailed);
				}

				buttonsProvider.OKButton.onClick += () => SetState(MenuState.ChangePassword);
				break;
			}
			case MenuState.AuthorizationFailed:
			{
				if (buttonsProvider != null)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
				break;
			}
			case MenuState.Registration:
			{
				if (pageController != null)
				{
					pageController.OnSuccess = () => SetState(MenuState.RegistrationSuccess);
					pageController.OnError = _ => SetState(MenuState.Main);
				}
				break;
			}
			case MenuState.RegistrationSuccess:
			{
				if (buttonsProvider != null)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
				break;
			}
			case MenuState.ChangePassword:
			{
				if (pageController != null)
				{
					pageController.OnSuccess = () => SetState(MenuState.ChangePasswordSuccess);
					pageController.OnError = _ => SetState(MenuState.Authorization);
				}
				break;
			}
			case MenuState.ChangePasswordSuccess:
			{
				if (buttonsProvider)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
				break;
			}
		}
		
		ClearTrace();
	}
}
