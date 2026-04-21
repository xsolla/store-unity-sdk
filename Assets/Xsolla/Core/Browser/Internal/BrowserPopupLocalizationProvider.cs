#if UNITY_WEBGL
using UnityEngine;

namespace Xsolla.Core
{
	internal class BrowserPopupLocalizationProvider
	{
		private const int MessageRow = 1;
		private const int ContinueButtonRow = 3;
		private const int CancelButtonRow = 4;

		private const string FallbackMessage = "Open a payment page in a new tab?";
		private const string FallbackContinueButtonText = "Yes, open";
		private const string FallbackCancelButtonText = "Cancel";

		private static CsvLocalizationParser LocalizationParser { get; set; }

		public BrowserPopupLocalizationProvider()
		{
			if (LocalizationParser != null)
				return;

			var asset = Resources.Load<TextAsset>("XsollaBrowserPopupLocalization");
			LocalizationParser = new CsvLocalizationParser(asset);
		}

		public static string GetMessageText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, MessageRow, FallbackMessage);

		public static string GetContinueButtonText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, ContinueButtonRow, FallbackContinueButtonText);

		public static string GetCancelButtonText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, CancelButtonRow, FallbackCancelButtonText);
	}
}
#endif