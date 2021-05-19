using System;
using Xsolla.Core;

public static class PriceFormatter
{
	private static string _lastCurrency;
	private const string DEFAULT_CURRENCY = "$";

	private static string LastCurrency
	{
		get { return _lastCurrency ?? DEFAULT_CURRENCY; }
		set { _lastCurrency = value; }
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
		
		var roundDownPrice = Math.Round((decimal)price, 2, MidpointRounding.AwayFromZero);

		return string.Format("{0}{1:F2}", outputCurrency, roundDownPrice);
	}
}
