using UnityEngine;
using Xsolla.Core;

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
				buttonsProvider.DemoUserButton.onClick += () =>
				{
					DemoController.Instance.GetImplementation().ValidateToken(
						DemoController.Instance.GetImplementation().GetDemoUserToken(), token =>
						{
							DemoController.Instance.GetImplementation().Token = token;
							DemoController.Instance.GetImplementation().SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, token);
							SetState(MenuState.Main);
						});
				};
		}

		switch (state)
		{
			case MenuState.Authorization:
			{
				if (pageController != null)
				{
					pageController.OnSuccess = () => SetState(MenuState.Main);
						pageController.OnError = err =>
						{
							var obj = SetState(MenuState.AuthorizationFailed);
							if (obj != null)
								obj.GetComponent<LoginPageErrorShower>()?.ShowError(err);
						};
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
					pageController.OnError = err =>
					{
						var obj = SetState(MenuState.RegistrationFailed);
						if(obj != null)
							obj.GetComponent<LoginPageErrorShower>()?.ShowError(err);
					};
				}
				break;
			}
			case MenuState.RegistrationSuccess:
			{
				if (buttonsProvider != null)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
				break;
			}
			case MenuState.RegistrationFailed:
			{
				if (buttonsProvider != null)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Registration);
				break;
			}
			case MenuState.ChangePassword:
			{
				if (pageController != null)
				{
					pageController.OnSuccess = () => SetState(MenuState.ChangePasswordSuccess);
					pageController.OnError = err =>
					{
						var obj = SetState(MenuState.ChangePasswordFailed);
						if (obj != null)
							obj.GetComponent<LoginPageErrorShower>()?.ShowError(err);
					};
				}
				break;
			}
			case MenuState.ChangePasswordSuccess:
			{
				if (buttonsProvider)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
				break;
			}
			case MenuState.ChangePasswordFailed:
			{
				if (buttonsProvider != null)
					buttonsProvider.OKButton.onClick += () => SetState(MenuState.ChangePassword);
				break;
			}
		}
		
		ClearTrace();
	}
}
