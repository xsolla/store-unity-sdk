#if UNITY_WEBGL
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class WidgetOpenConfirmationPopupLocalizationProvider
	{
		private const int MessageRow = 1;
		private const int ContinueButtonRow = 2;
		private const int CancelButtonRow = 3;

		private const string FallbackMessage = "Open a new tab to log in?";
		private const string FallbackContinueButtonText = "Yes, open";
		private const string FallbackCancelButtonText = "Cancel";

		private static CsvLocalizationParser LocalizationParser { get; set; }

		public WidgetOpenConfirmationPopupLocalizationProvider()
		{
			if (LocalizationParser != null)
				return;

			var asset = Resources.Load<TextAsset>("XsollaWidgetOpenConfirmationPopupLocalization");
			LocalizationParser = new CsvLocalizationParser(asset);
		}

		public string GetMessageText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, MessageRow, FallbackMessage);

		public string GetContinueButtonText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, ContinueButtonRow, FallbackContinueButtonText);

		public string GetCancelButtonText(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, CancelButtonRow, FallbackCancelButtonText);
	}
}
#endif