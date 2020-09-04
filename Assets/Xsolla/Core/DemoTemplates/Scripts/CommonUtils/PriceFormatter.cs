using Xsolla.Core;

public static class PriceFormatter
{
	private static string _lastCurrency;
	private const string DEFAULT_CURRENCY = "$";

	private static string LastCurrency
	{
		get => _lastCurrency ?? DEFAULT_CURRENCY;
		set => _lastCurrency = value;
	}

	public static string FormatPrice(float price)
	{
		return FormatPrice(LastCurrency, price);
	}

	public static string FormatPrice(string currency, float price)
	{
		var currencySymbol = RegionalCurrency.GetCurrencySymbol(currency);
		var outputCurrency = string.IsNullOrEmpty(currencySymbol) ? currency : currencySymbol;
		LastCurrency = outputCurrency;
		
		var roundDownPrice = RoundDown(price);

		return $"{outputCurrency}{roundDownPrice:F2}";
	}

	private static float RoundDown(float value)
	{
		//example: value 1.4958f
		var multipliedValue = value * 100f;//149.58f
		var convertedToInt = (int)multipliedValue;//149
		var convertedBackToFloat = (float)convertedToInt;//149.0f
		var result = convertedBackToFloat * 0.01f;//1.49f

		return result;
	}
}
