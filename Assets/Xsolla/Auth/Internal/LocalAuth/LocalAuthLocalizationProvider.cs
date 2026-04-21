#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class LocalAuthLocalizationProvider
	{
		private const int SuccessTitleRow = 1;
		private const int SuccessMessageRow = 2;
		private const int ErrorTitleRow = 3;
		private const int ErrorMessageRow = 4;

		private const string FallbackSuccessTitle = "Successful login";
		private const string FallbackSuccessMessage = "You can close this tab and return to the game";
		private const string FallbackErrorTitle = "Unsuccessful login";
		private const string FallbackErrorMessage = "Close this tab and try to log in again";

		private static CsvLocalizationParser LocalizationParser { get; set; }

		public LocalAuthLocalizationProvider()
		{
			if (LocalizationParser != null)
				return;

			var asset = Resources.Load<TextAsset>("XsollaLocalAuthLocalization");
			LocalizationParser = new CsvLocalizationParser(asset);
		}

		public string GetSuccessTitle(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, SuccessTitleRow, FallbackSuccessTitle);

		public string GetSuccessMessage(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, SuccessMessageRow, FallbackSuccessMessage);

		public string GetErrorTitle(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, ErrorTitleRow, FallbackErrorTitle);

		public string GetErrorMessage(string locale)
			=> LocalizationParser.GetCellTextOrFallback(locale, ErrorMessageRow, FallbackErrorMessage);
	}
}
#endif