using System.Collections.Generic;
using Xsolla.Catalog;

namespace Xsolla.ReadyToUseStore
{
	internal class WarmupHelper
	{
		public static void WarmupCatalogImages()
		{
			XsollaCatalog.GetItems(
				items => {
					var urls = FetchAllImageUrls(items);
					foreach (var url in urls)
					{
						SpriteCache.Get(url, null);
					}
				},
				null);
		}

		public static List<string> FetchAllImageUrls(StoreItems items)
		{
			var result = new List<string>();

			foreach (var item in items.items)
			{
				result.Add(item.image_url);

				foreach (var virtualPrice in item.virtual_prices)
				{
					result.Add(virtualPrice.image_url);
				}
			}

			return result;
		}
	}
}