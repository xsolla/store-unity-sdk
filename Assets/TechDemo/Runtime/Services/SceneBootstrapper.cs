using UnityEngine;
using Xsolla.Core;
using Xsolla.TechDemo.Scenarios;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class SceneBootstrapper : MonoBehaviour
	{
		[field: SerializeField] private UiRoot UiRoot { get; set; }

		private void Start()
		{
			SetupSettings();

			var serviceLocator = new ServiceLocator(UiRoot, this);
			RegisterScenarios(serviceLocator.ScenarioRegistry);

			UiRoot.Construct(serviceLocator, ScenarioGroup.Store);
		}

		private static void SetupSettings()
		{
			XsollaSettings.StoreProjectId = Constants.DEFAULT_PROJECT_ID;
			XsollaSettings.LoginId = Constants.DEFAULT_LOGIN_ID;
			XsollaSettings.OAuthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;
		}

		private static void RegisterScenarios(ScenarioRegistry registry)
		{
			new LoginScenariosRegistrar(registry).RegisterScenarios();
			new StoreScenariosRegistrar(registry).RegisterScenarios();
		}
	}
}