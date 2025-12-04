using System.Globalization;
using UnityEngine;

namespace Xsolla.Core
{
	public static class LocaleUtil
	{
		public static string GetSystemLanguageCode()
		{
			var rawCode = GetLanguageCodeRaw();
			if (string.IsNullOrEmpty(rawCode))
				return "en";

			rawCode = rawCode
				.Replace("_", "-")
				.ToLowerInvariant();

			if (rawCode.StartsWith("zh"))
				rawCode = PostProcessChineseLanguageCode(rawCode);

			return rawCode;
		}

		private static string GetLanguageCodeRaw()
		{
			var fromUnity = GetLanguageCodeFromUnity();
			return string.IsNullOrEmpty(fromUnity)
				? GetLanguageCodeFromCultureInfo()
				: fromUnity;
		}

		private static string PostProcessChineseLanguageCode(string code)
		{
			if (string.IsNullOrEmpty(code))
				return code;

			const string simplifiedResult = "zh_hans";
			const string traditionalResult = "zh_hant_tw";

			// exact match for "zh" 
			if (code == "zh")
				return simplifiedResult;

			// quick checks for common prefixes
			if (code.StartsWith("zh_hans"))
				return simplifiedResult;

			if (code.StartsWith("zh_hant"))
				return traditionalResult;

			// region based checks
			switch (code)
			{
				case "zh_cn":
				case "zh_sg":
					return simplifiedResult;
				case "zh_tw":
				case "zh_hk":
				case "zh_mo":
					return traditionalResult;
			}

			// Weird cases where wrong script is appended to a region
			// Example: "zh_hans_hk" from buggy OS locale
			if (code.EndsWith("_hk") || code.EndsWith("_mo") || code.EndsWith("_tw"))
				return traditionalResult;

			if (code.EndsWith("_cn") || code.EndsWith("_sg"))
				return simplifiedResult;

			// if nothing matched — return simplified as default
			return simplifiedResult;
		}

		private static string GetLanguageCodeFromUnity()
		{
			var language = Application.systemLanguage;
			return language switch {
				SystemLanguage.Afrikaans          => "af",
				SystemLanguage.Arabic             => "ar",
				SystemLanguage.Basque             => "eu",
				SystemLanguage.Belarusian         => "be",
				SystemLanguage.Bulgarian          => "bg",
				SystemLanguage.Catalan            => "ca",
				SystemLanguage.Chinese            => "zh",
				SystemLanguage.Czech              => "cs",
				SystemLanguage.Danish             => "da",
				SystemLanguage.Dutch              => "nl",
				SystemLanguage.English            => "en",
				SystemLanguage.Estonian           => "et",
				SystemLanguage.Faroese            => "fo",
				SystemLanguage.Finnish            => "fi",
				SystemLanguage.French             => "fr",
				SystemLanguage.German             => "de",
				SystemLanguage.Greek              => "el",
				SystemLanguage.Hebrew             => "he",
				SystemLanguage.Hungarian          => "hu",
				SystemLanguage.Icelandic          => "is",
				SystemLanguage.Indonesian         => "id",
				SystemLanguage.Italian            => "it",
				SystemLanguage.Japanese           => "ja",
				SystemLanguage.Korean             => "ko",
				SystemLanguage.Latvian            => "lv",
				SystemLanguage.Lithuanian         => "lt",
				SystemLanguage.Norwegian          => "no",
				SystemLanguage.Polish             => "pl",
				SystemLanguage.Portuguese         => "pt",
				SystemLanguage.Romanian           => "ro",
				SystemLanguage.Russian            => "ru",
				SystemLanguage.SerboCroatian      => "sh",
				SystemLanguage.Slovak             => "sk",
				SystemLanguage.Slovenian          => "sl",
				SystemLanguage.Spanish            => "es",
				SystemLanguage.Swedish            => "sv",
				SystemLanguage.Thai               => "th",
				SystemLanguage.Turkish            => "tr",
				SystemLanguage.Ukrainian          => "uk",
				SystemLanguage.Vietnamese         => "vi",
				SystemLanguage.ChineseSimplified  => "zh-CN",
				SystemLanguage.ChineseTraditional => "zh-TW",
				_                                 => null
			};
		}

		private static string GetLanguageCodeFromCultureInfo()
		{
			return CultureInfo.CurrentCulture.Name;
		}
	}
}