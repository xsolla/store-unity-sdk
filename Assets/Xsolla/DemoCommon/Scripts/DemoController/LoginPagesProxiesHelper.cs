namespace Xsolla.Demo
{
	public class LoginPagesProxiesHelper : OnStateChangedHandler
	{
		protected sealed override void OnStateChanged(MenuState lastState, MenuState newState)
		{
			if (newState == MenuState.Authorization)
			{
				var proxyScript = FindObjectOfType<LoginProxyActionHolder>();
				var loginEnterScript = base.StateMachine.StateObject.GetComponent<LoginPageEnterController>();

				if (proxyScript != null)
				{
					if (loginEnterScript != null)
						loginEnterScript.RunLoginProxyAction(proxyScript.ProxyAction, proxyScript.ProxyActionArgument);

					Destroy(proxyScript.gameObject);
				}
			}
			else if (newState == MenuState.LoginSettingsError)
			{
				var proxyScript = FindObjectOfType<LoginSettingsErrorHolder>();
				var errorShower = base.StateMachine.StateObject.GetComponent<LoginPageErrorShower>();

				if (proxyScript != null)
				{
					if (errorShower != null)
						errorShower.ShowError(proxyScript.LoginSettingsError);

					Destroy(proxyScript.gameObject);
				}
			}
		}
	}
}
