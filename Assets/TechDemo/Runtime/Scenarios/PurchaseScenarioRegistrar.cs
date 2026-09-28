using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo.Scenarios
{
	public class PurchaseScenarioRegistrar
	{
		private readonly ScenarioRegistry ScenarioRegistry;

		public PurchaseScenarioRegistrar(ScenarioRegistry scenarioRegistry)
		{
			ScenarioRegistry = scenarioRegistry;
		}

		public void RegisterScenarios()
		{
			// Common
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByInternalBrowserSandbox>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByInternalBrowserLive>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByExternalBrowserSandbox>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByExternalBrowserLive>(ScenarioGroup.Purchase);

			// Android specific
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByCustomTabsSandbox>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByCustomTabsLive>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByWebViewSandbox>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByWebViewLive>(ScenarioGroup.Purchase);
			ScenarioRegistry.RegisterScenarioType<CompletesItemPurchaseByTwaSandbox>(ScenarioGroup.Purchase);
		}
	}
}