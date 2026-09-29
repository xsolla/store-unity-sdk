namespace Xsolla.Demo
{
	public class InventoryServiceFactory
	{
		public IInventoryService Create(DebugConfig debugConfig)
		{
			var inventoryStorage = new InventoryStorage();
			IInventoryService inventoryService = new XsollaInventoryService(inventoryStorage);

#if DEBUG_XSOLLA_DEMO
			if (debugConfig.UseFakeInventoryAndStore)
			{
				inventoryStorage = debugConfig.FakeInventoryStorage;
				inventoryService = new FakeInventoryService(inventoryStorage);
			}
#endif

			return inventoryService;
		}
	}
}