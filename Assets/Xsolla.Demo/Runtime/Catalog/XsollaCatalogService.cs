using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class XsollaCatalogService : ICatalogService
	{
		private readonly CatalogStorage CatalogStorage;

		public XsollaCatalogService(CatalogStorage catalogStorage)
		{
			CatalogStorage = catalogStorage;
		}

		public IEnumerator FetchCatalogCoroutine()
		{
			CatalogStorage.Clear();
			yield return FetchVirtualItems();
			yield return FetchVirtualCurrencyPackages();
		}

		private IEnumerator FetchVirtualItems()
		{
			var isRequestDone = false;

			var storeItems = new List<StoreItem>();
			XsollaCatalog.GetItems(
				response => {
					storeItems.AddRange(response.items);
					isRequestDone = true;
				},
				_ => isRequestDone = true);

			yield return new WaitUntil(() => isRequestDone);
			yield return UpdateCatalog(storeItems);
		}

		private IEnumerator FetchVirtualCurrencyPackages()
		{
			var isRequestDone = false;

			var storeItems = new List<StoreItem>();
			XsollaCatalog.GetVirtualCurrencyPackagesList(
				response => {
					storeItems.AddRange(response.items);
					isRequestDone = true;
				},
				_ => isRequestDone = true);

			yield return new WaitUntil(() => isRequestDone);
			yield return UpdateCatalog(storeItems);
		}

		private IEnumerator UpdateCatalog(IEnumerable<StoreItem> storeItems)
		{
			foreach (var storeItem in storeItems)
			{
				var sku = storeItem.sku;
				var catalogItem = new CatalogRecord(sku, storeItem);
				CatalogStorage.AddItem(catalogItem);

				yield return SpriteCache.LoadSprite(
					storeItem.image_url,
					sprite => CatalogStorage.UpdateIcon(sku, sprite));

				foreach (var virtualPrice in storeItem.virtual_prices)
				{
					yield return SpriteCache.LoadSprite(virtualPrice.image_url, null);
				}
			}
		}

		public CatalogRecord GetItem(string sku)
		{
			return CatalogStorage.GetItem(sku);
		}
	}
}