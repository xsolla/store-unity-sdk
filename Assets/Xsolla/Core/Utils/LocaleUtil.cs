using System.Globalization;
using UnityEngine;

namespace Xsolla.Core
{
	public static class LocaleUtil
	{
		public static string GetSystemLanguageCode()
		{
			var raw = GetLanguageCodeRaw();
			return string.IsNullOrEmpty(raw)
				? "en"
				: raw.Replace("-", "_").ToLowerInvariant();
		}

		private static string GetLanguageCodeRaw()
		{
			var fromUnity = GetLanguageCodeFromUnity();
			return string.IsNullOrEmpty(fromUnity)
				? GetLanguageCodeFromCultureInfo()
				: fromUnity;
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