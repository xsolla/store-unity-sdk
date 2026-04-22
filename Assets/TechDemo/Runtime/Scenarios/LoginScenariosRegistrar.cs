using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo.Scenarios
{
	public class LoginScenariosRegistrar
	{
		private readonly ScenarioRegistry ScenarioRegistry;

		public LoginScenariosRegistrar(ScenarioRegistry scenarioRegistry)
		{
			ScenarioRegistry = scenarioRegistry;
		}

		public void RegisterScenarios()
		{
			// Widget
			ScenarioRegistry.RegisterScenarioType<AuthenticatesWithXsollaWidgetByExternalBrowser>(ScenarioGroup.Login);
			ScenarioRegistry.RegisterScenarioType<AuthenticatesWithXsollaWidgetByInternalBrowser>(ScenarioGroup.Login);

			// Google
			ScenarioRegistry.RegisterScenarioType<AuthenticatesViaGoogleByInternalBrowser>(ScenarioGroup.Login);
			ScenarioRegistry.RegisterScenarioType<AuthenticatesViaGoogleByExternalBrowser>(ScenarioGroup.Login);

			// Steam
			ScenarioRegistry.RegisterScenarioType<AuthenticatesViaSteamByInternalBrowser>(ScenarioGroup.Login);
			ScenarioRegistry.RegisterScenarioType<AuthenticatesViaSteamByExternalBrowser>(ScenarioGroup.Login);

			// Other
			ScenarioRegistry.RegisterScenarioType<AuthenticatesViaDeviceId>(ScenarioGroup.Login);
		}
	}
}