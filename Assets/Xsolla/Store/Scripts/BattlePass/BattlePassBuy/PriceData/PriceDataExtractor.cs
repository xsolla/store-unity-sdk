using System.Collections.Generic;

namespace Xsolla.Demo
{
    public class PriceDataExtractor
    {
		private KeyValuePair<string, string>? _lastCurrencySkuImage;

		public PriceData ExtractPriceData(CatalogItemModel catalogItemModel)
		{
			var result = default(PriceData);

			if (catalogItemModel.RealPrice.HasValue)
			{
				var realPriceData = new RealPriceData();
				realPriceData.currency = catalogItemModel.RealPrice.Value.Key;
				realPriceData.price = catalogItemModel.RealPrice.Value.Value;
				result = realPriceData;
			}
			else if (catalogItemModel.VirtualPrice.HasValue)
			{
				var virtualPriceData = new VirtualPriceData();
				var currencySku = catalogItemModel.VirtualPrice.Value.Key;

				virtualPriceData.currencySku = currencySku;
				virtualPriceData.price = (int)catalogItemModel.VirtualPrice.Value.Value;
				virtualPriceData.currencyImageUrl = GetCurrencyImageUrl(currencySku);

				result = virtualPriceData;
			}
			else
				Debug.LogError($"Catalog item with sku: '{catalogItemModel.Sku}' has neither real or virtual price on it.");

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
