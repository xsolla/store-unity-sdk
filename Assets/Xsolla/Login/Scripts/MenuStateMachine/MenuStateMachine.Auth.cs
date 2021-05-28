using UnityEngine;

namespace Xsolla.Demo
{
	public partial class MenuStateMachine : MonoBehaviour
	{
		private bool IsAuthInProgress => _stateObject.GetComponent<LoginPageEnterController>()?.IsAuthInProgress ?? false;

		private void CheckAuthorizationState(MenuState state, GameObject go)
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
						buttonsProvider.LogInButton.onClick += () =>
						{
							if (IsAuthInProgress)
								return;
							else
								SetState(MenuState.Registration);
						};
					else
						buttonsProvider.LogInButton.onClick += () => SetState(MenuState.Authorization);
				}
				if (buttonsProvider.DemoUserButton != null)
					buttonsProvider.DemoUserButton.onClick += () =>
					{
						if (IsAuthInProgress)
							return;

						var proxyObject = new GameObject();
						var proxyScript = proxyObject.AddComponent<LoginProxyActionHolder>();

						proxyScript.ProxyAction = LoginProxyActions.RunDemoUserAuthDelegate;
						proxyScript.ProxyActionArgument = null;

						//State machine does not handle switching to the same state so pressing DemoUserButton while
						//in MenuState.Authorization results in failure, this is a workaround
						if (state == MenuState.Authorization)
							DemoController.Instance.SetState(MenuState.RegistrationSuccess);

						DemoController.Instance.SetState(MenuState.Authorization);
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

					buttonsProvider.OKButton.onClick += () =>
					{ 
						if (IsAuthInProgress)
							return;
						else
							SetState(MenuState.ChangePassword);
					};
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
}
