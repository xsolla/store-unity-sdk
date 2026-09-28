using UnityEngine;
using Xsolla.TechDemo.Scenarios;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class SceneBootstrapper : MonoBehaviour
	{
		[field: SerializeField] private UiRoot UiRoot { get; set; }

		private void Start()
		{
			var serviceLocator = new ServiceLocator(UiRoot, this);
			RegisterScenarios(serviceLocator.ScenarioRegistry);

			UiRoot.Construct(serviceLocator, ScenarioGroup.Store);
		}

		private static void RegisterScenarios(ScenarioRegistry registry)
		{
			new LoginScenariosRegistrar(registry).RegisterScenarios();
			new StoreScenariosRegistrar(registry).RegisterScenarios();
			new PurchaseScenarioRegistrar(registry).RegisterScenarios();
		}
	}
}