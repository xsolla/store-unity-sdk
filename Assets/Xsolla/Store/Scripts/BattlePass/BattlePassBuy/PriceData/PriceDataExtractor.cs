using System.Collections.Generic;

namespace Xsolla.Demo
{
    public class PriceDataExtractor
    {
		private KeyValuePair<string, KeyValuePair<string, string>>? _lastCurrencySkuImage;

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

				TryGetCurrencyInfo(currencySku, out string currencyName, out string currencyImageUrl);
				virtualPriceData.currencyName = currencyName;
				virtualPriceData.currencyImageUrl = currencyImageUrl;

				result = virtualPriceData;
			}
			else
				Debug.LogError($"Catalog item with sku: '{catalogItemModel.Sku}' has neither real or virtual price on it.");

			return result;
		}

		private bool TryGetCurrencyInfo(string targetSku, out string currencyName, out string currencyImageUrl)
		{
			if (_lastCurrencySkuImage.HasValue && _lastCurrencySkuImage.Value.Key == targetSku)
			{
				var infoPair = _lastCurrencySkuImage.Value.Value;
				currencyName = infoPair.Key;
				currencyImageUrl = infoPair.Value;
				return true;
			}
			else
			{
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
					currencyName = targetCurrency.Name;
					currencyImageUrl = targetCurrency.ImageUrl;

					var infoPair = new KeyValuePair<string,string>(currencyName, currencyImageUrl);

					_lastCurrencySkuImage = new KeyValuePair<string, KeyValuePair<string, string>>(targetSku, infoPair);
					return true;
				}
				else
				{
					Debug.LogError($"Could not find currency with sku: '{targetSku}'");
					currencyName = null;
					currencyImageUrl = null;
					return false;
				}
			}
		}
	}
}
