using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class LocalAuthLocalizationProvider
	{
		private static TextAsset _csvAsset;
		private static TextAsset CsvAsset => _csvAsset ??= Resources.Load<TextAsset>("XsollaLocalAuthLocalization");

		private readonly CsvReader CsvReader = new();

		private const int SuccessTitleRow = 1;
		private const int SuccessMessageRow = 2;
		private const int ErrorTitleRow = 3;
		private const int ErrorMessageRow = 4;

		private const string FallbackSuccessTitle = "Successful login";
		private const string FallbackSuccessMessage = "You can close this tab and return to the game";
		private const string FallbackErrorTitle = "Unsuccessful login";
		private const string FallbackErrorMessage = "Close this tab and try to log in again";

		public string GetSuccessTitle(string locale)
			=> GetLocalizedOrFallback(locale, SuccessTitleRow, FallbackSuccessTitle);

		public string GetSuccessMessage(string locale)
			=> GetLocalizedOrFallback(locale, SuccessMessageRow, FallbackSuccessMessage);

		public string GetErrorTitle(string locale)
			=> GetLocalizedOrFallback(locale, ErrorTitleRow, FallbackErrorTitle);

		public string GetErrorMessage(string locale)
			=> GetLocalizedOrFallback(locale, ErrorMessageRow, FallbackErrorMessage);

		private string GetLocalizedOrFallback(string locale, int row, string fallback)
		{
			if (!CsvAsset)
				return fallback;

			var csvContent = CsvAsset.text;
			if (string.IsNullOrEmpty(csvContent))
				return fallback;

			var column = CsvReader.GetLocaleColumn(csvContent, locale);
			var cellText = CsvReader.GetCellText(csvContent, column, row);

			return string.IsNullOrEmpty(cellText)
				? fallback
				: cellText;
		}
	}
}