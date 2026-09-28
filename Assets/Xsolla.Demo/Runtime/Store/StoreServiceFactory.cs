namespace Xsolla.Demo
{
	public class StoreServiceFactory
	{
		public IStoreService Create(DebugConfig debugConfig)
		{
			IStoreService storeService = new XsollaStoreService();

#if DEBUG_XSOLLA_DEMO
			if (debugConfig.UseFakeInventoryAndStore)
			{
				var inventoryStorage = debugConfig.FakeInventoryStorage;
				storeService = new DebugStoreService(inventoryStorage);
			}
#endif

			return storeService;
		}
	}
}