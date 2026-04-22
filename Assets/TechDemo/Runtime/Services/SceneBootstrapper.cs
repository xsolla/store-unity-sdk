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

			var serviceLocator = new ServiceLocator(UiRoot);
			RegisterScenarios(serviceLocator.ScenarioRegistry);

			UiRoot.Construct(serviceLocator);
		}

		private static void SetupSettings()
		{
			XsollaSettings.StoreProjectId = Constants.DEFAULT_PROJECT_ID;
			XsollaSettings.LoginId = Constants.DEFAULT_LOGIN_ID;
			XsollaSettings.OAuthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;
		}

		private static void RegisterScenarios(ScenarioRegistry registry)
		{
			registry.RegisterScenarioType<SampleLoginScenarioA>(ScenarioGroup.Login);
			registry.RegisterScenarioType<SampleLoginScenarioB>(ScenarioGroup.Login);
			registry.RegisterScenarioType<AuthByXsollaUser>(ScenarioGroup.Login);

			registry.RegisterScenarioType<PurchaseItem>(ScenarioGroup.Store);
			registry.RegisterScenarioType<SampleStoreScenarioA>(ScenarioGroup.Store);
			registry.RegisterScenarioType<SampleStoreScenarioB>(ScenarioGroup.Store);
		}
	}
}