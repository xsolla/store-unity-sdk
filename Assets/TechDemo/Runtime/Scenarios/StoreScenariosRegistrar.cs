using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo.Scenarios
{
	public class StoreScenariosRegistrar
	{
		private readonly ScenarioRegistry ScenarioRegistry;
		
		public StoreScenariosRegistrar(ScenarioRegistry scenarioRegistry)
		{
			ScenarioRegistry = scenarioRegistry;
		}
		
		public void RegisterScenarios()
		{
			// Inventory
			ScenarioRegistry.RegisterScenarioType<IncreasesVirtualCurrencyBalanceAfterPurchasingCurrencyPack>(ScenarioGroup.Store);
			ScenarioRegistry.RegisterScenarioType<GrantsPersonalizedItemToUserInventoryAfterPurchase>(ScenarioGroup.Store);
			ScenarioRegistry.RegisterScenarioType<ConsumesConsumableItemAndUpdatesQuantity>(ScenarioGroup.Store);
			ScenarioRegistry.RegisterScenarioType<GrantsAllBundleItemsAfterPurchase>(ScenarioGroup.Store);
			
			// Cart
			ScenarioRegistry.RegisterScenarioType<CompletesPurchaseOfSingleItemThroughCart>(ScenarioGroup.Store);
			ScenarioRegistry.RegisterScenarioType<CompletesPurchaseOfMultipleDifferentItemsThroughCart>(ScenarioGroup.Store);
			ScenarioRegistry.RegisterScenarioType<CompletesPurchaseOfMultipleSameItemsThroughCart>(ScenarioGroup.Store);
			
			// Settings
			ScenarioRegistry.RegisterScenarioType<AppliesConfiguredPayStationThemeAndRedirectPolicyDuringCheckout>(ScenarioGroup.Store);
		}
	}
}