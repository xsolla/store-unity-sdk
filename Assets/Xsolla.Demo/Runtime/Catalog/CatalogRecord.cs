using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CatalogRecord
	{
		public CatalogRecord(string sku, StoreItem storeItem)
		{
			Sku = sku;
			StoreItem = storeItem;
		}

		public string Sku { get; }

		public Sprite IconSprite { get; private set; }

		private StoreItem StoreItem { get; }

		public string GetName()
		{
			return StoreItem.name;
		}

		public void UpdateIconSprite(Sprite sprite)
		{
			IconSprite = sprite;
		}

		public bool IsPurchaseByVirtualCurrency()
		{
			return StoreItem.virtual_prices.Length > 0;
		}

		public string GetFiatCurrency()
		{
			return StoreItem.price.currency;
		}

		public string GetFiatPurchasePrice()
		{
			return StoreItem.price.amount.ToLowerInvariant();
		}

		public string GetVirtualCurrencySku()
		{
			return StoreItem.virtual_prices[0].sku;
		}

		public Sprite GetVirtualCurrencyIconSprite()
		{
			var price = StoreItem.virtual_prices[0].image_url;
			return SpriteCache.GetSpriteFromUrl(price);
		}

		public string GetVirtualCurrencyAmount()
		{
			return StoreItem.virtual_prices[0].amount.ToLowerInvariant();
		}
	}
}