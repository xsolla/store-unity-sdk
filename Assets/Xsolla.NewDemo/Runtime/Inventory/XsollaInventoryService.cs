using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Demo
{
	public class XsollaInventoryService : IInventoryService
	{
		private readonly InventoryStorage InventoryStorage;

		public XsollaInventoryService(InventoryStorage inventoryStorage)
		{
			InventoryStorage = inventoryStorage;
		}

		public IEnumerator FetchInventoryCoroutine()
		{
			InventoryStorage.Clear();

			var isRequestDone = false;
			var inventoryItems = new List<InventoryItem>();

			XsollaInventory.GetInventoryItems(
				response => {
					inventoryItems.AddRange(response.items);
					isRequestDone = true;
				},
				error => {
					XDebug.LogError(error.errorMessage);
					isRequestDone = true;
				});

			yield return new WaitUntil(() => isRequestDone);

			foreach (var inventoryItem in inventoryItems)
			{
				var sku = inventoryItem.sku;
				var quantity = inventoryItem.quantity ?? 0;
				InventoryStorage.UpdateQuantity(sku, quantity);

				var iconUrl = inventoryItem.image_url;
				yield return SpriteCache.LoadSprite(iconUrl, sprite => InventoryStorage.UpdateIconSprite(sku, sprite));
			}
		}

		public IEnumerable<InventoryRecord> GetAllItems()
			=> InventoryStorage.GetAllItems();

		public InventoryRecord GetItem(string sku)
			=> InventoryStorage.GetItem(sku);
	}
}