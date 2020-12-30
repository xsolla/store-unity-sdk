using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Core
{
	public static class RegionalCurrency
	{
		class CurrencyProperties
		{
			public string Symbol { get; set; }
			public string Code { get; set; }
		}

		static readonly Dictionary<SystemLanguage, CurrencyProperties> Currencies =
			new Dictionary<SystemLanguage, CurrencyProperties>()
			{
				{SystemLanguage.Chinese, new CurrencyProperties {Symbol = "元", Code = "CNY"}},
				{SystemLanguage.English, new CurrencyProperties {Symbol = "$", Code = "USD"}},
				{SystemLanguage.French, new CurrencyProperties {Symbol = "€", Code = "EUR"}},
				{SystemLanguage.German, new CurrencyProperties {Symbol = "€", Code = "EUR"}},
				{SystemLanguage.Korean, new CurrencyProperties {Symbol = "₩", Code = "KRW"}},
				{SystemLanguage.Portuguese, new CurrencyProperties {Symbol = "R$", Code = "BRL"}},
				{SystemLanguage.Russian, new CurrencyProperties {Symbol = "₽", Code = "RUB"}},
				{SystemLanguage.Spanish, new CurrencyProperties {Symbol = "€", Code = "EUR"}},
				{SystemLanguage.Unknown, new CurrencyProperties {Symbol = "$", Code = "USD"}}
			};

		public static string CurrencyCode
		{
			get
			{
				if (Currencies.ContainsKey(Application.systemLanguage))
				{
					return Currencies[Application.systemLanguage].Code;
				}

				return Currencies[SystemLanguage.Unknown].Code;
			}
		}

		public static string CurrencySymbol
		{
			get
			{
				if (Currencies.ContainsKey(Application.systemLanguage))
				{
					return Currencies[Application.systemLanguage].Symbol;
				}

				return Currencies[SystemLanguage.Unknown].Symbol;
			}
		}

		public static string GetCurrencySymbol(string code)
		{
			if (Currencies.Any(c => c.Value.Code == code))
			{
				return Currencies.First(c => c.Value.Code == code).Value.Symbol;
			}

			return string.Empty;
		}
	}
}