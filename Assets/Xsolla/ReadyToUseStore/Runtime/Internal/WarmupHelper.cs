using System.Collections.Generic;
using System.Linq;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class WarmupHelper
	{
		public static void WarmupCatalogImages()
		{
			XsollaCatalog.GetItems(
				items => {
					var urls = FetchAllImageUrls(items.items);
					foreach (var url in urls)
					{
						SpriteCache.Get(url, null);
					}
				},
				null,
				sdkType: SdkType.ReadyToUseStore);

			XsollaCatalog.GetVirtualCurrencyPackagesList(
				packages => {
					var storeItems = packages.items.Select(x => x as StoreItem).ToArray();
					var urls = FetchAllImageUrls(storeItems);
					foreach (var url in urls)
					{
						SpriteCache.Get(url, null);
					}
				},
				null,
				sdkType: SdkType.ReadyToUseStore);
		}

		public static List<string> FetchAllImageUrls(StoreItem[] items)
		{
			var result = new List<string>();

			foreach (var item in items)
			{
				result.Add(item.image_url);

				foreach (var virtualPrice in item.virtual_prices)
				{
					result.Add(virtualPrice.image_url);
				}
			}

			return result.Distinct().ToList();
		}
	}
}