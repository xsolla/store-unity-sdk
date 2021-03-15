using System.Collections.Generic;

namespace Xsolla.Demo
{
    public class LevelUpPriceDataExtractor
    {
		private KeyValuePair<string, string>? _lastCurrencySkuImage;

		public LevelUpPriceData ExtractPriceData(CatalogItemModel levelUpUtil)
		{
			var result = default(LevelUpPriceData);

			if (levelUpUtil.RealPrice.HasValue)
			{
				var realPriceData = new LevelUpRealPriceData();
				realPriceData.currency = levelUpUtil.RealPrice.Value.Key;
				realPriceData.price = levelUpUtil.RealPrice.Value.Value;
				result = realPriceData;
			}
			else if (levelUpUtil.VirtualPrice.HasValue)
			{
				var virtualPriceData = new LevelUpVirtualPriceData();
				var currencySku = levelUpUtil.VirtualPrice.Value.Key;

				virtualPriceData.currencySku = currencySku;
				virtualPriceData.price = (int)levelUpUtil.VirtualPrice.Value.Value;
				virtualPriceData.currencyImageUrl = GetCurrencyImageUrl(currencySku);

				result = virtualPriceData;
			}
			else
				Debug.LogError($"Level up utils with sku: '{levelUpUtil.Sku}' has neither real or virtual price on it.");

			return result;
		}

		private string GetCurrencyImageUrl(string targetSku)
		{
			if (_lastCurrencySkuImage.HasValue && _lastCurrencySkuImage.Value.Key == targetSku)
				return _lastCurrencySkuImage.Value.Value;

			var targetCurrency = default(VirtualCurrencyModel);
			foreach (var item in UserCatalog.Instance.VirtualCurrencies)
			{
				if (item.Sku == targetSku)
				{
					targetCurrency = item;
					break;
				}
			}

			if (targetCurrency != null)
			{
				_lastCurrencySkuImage = new KeyValuePair<string, string>(targetSku, targetCurrency.ImageUrl);
				return targetCurrency.ImageUrl;
			}
			else
			{
				Debug.LogError($"Could not find currency with sku: '{targetSku}'");
				return null;
			}
		}
	}
}
