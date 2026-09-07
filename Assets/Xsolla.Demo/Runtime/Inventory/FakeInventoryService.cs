#if DEBUG_XSOLLA_DEMO
using System.Collections;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class FakeInventoryService : IInventoryService
	{
		private readonly InventoryStorage InventoryStorage;

		public FakeInventoryService(InventoryStorage inventoryStorage)
		{
			InventoryStorage = inventoryStorage;
		}

		public IEnumerator FetchInventoryCoroutine()
		{
			yield return LoadIconSprite("heart", "https://cdn3.xsolla.com/img/misc/images/09feda1a1d4af0883c88db708246ad72.png");
			yield return LoadIconSprite("owl", "https://cdn3.xsolla.com/img/misc/images/037a4ec60d88ef24096b968d2b167204.png");
			yield return LoadIconSprite("coin", "https://cdn3.xsolla.com/img/misc/images/7900d08429e6fb1f496cd29f846f6005.png");
		}

		private IEnumerator LoadIconSprite(string sku, string url)
		{
			yield return SpriteCache.LoadSprite(url, sprite => InventoryStorage.UpdateIconSprite(sku, sprite));
		}

		public IEnumerable<InventoryRecord> GetAllItems()
			=> InventoryStorage.GetAllItems();

		public InventoryRecord GetItem(string sku)
			=> InventoryStorage.GetItem(sku);
	}
}
#endif