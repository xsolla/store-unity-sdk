#if UNITY_WEBGL
using System.Text.RegularExpressions;
using UnityEngine;

namespace Xsolla.Auth
{
	internal class WidgetOpenConfirmationPopupLocalizationDataProvider
	{
		private static TextAsset _dataAsset;

		private static TextAsset DataAsset
		{
			get
			{
				if (!_dataAsset)
					_dataAsset = Resources.Load<TextAsset>("WidgetOpenConfirmationPopupLocalization");

				return _dataAsset;
			}
		}

		private const string DefaultMessage = "Open a new tab to log in?";
		private const string DefaultContinueButtonText = "Yes, open";
		private const string DefaultCancelButtonText = "Cancel";

		private const int MessageLineIndex = 1;
		private const int ContinueButtonLineIndex = 2;
		private const int CancelButtonLineIndex = 3;

		public string GetMessageText(string locale)
		{
			if (string.IsNullOrEmpty(locale))
				return DefaultMessage;

			var data = DataAsset.text;
			if (string.IsNullOrEmpty(data))
				return DefaultMessage;

			var localeColumn = GetLocaleColumn(data, locale);
			var cellText = GetCellText(data, localeColumn, MessageLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultMessage
				: cellText;
		}

		public string GetContinueButtonText(string locale)
		{
			var dataText = DataAsset.text;
			if (string.IsNullOrEmpty(dataText))
				return DefaultContinueButtonText;

			var rowIndex = GetLocaleColumn(dataText, locale);
			var cellText = GetCellText(dataText, rowIndex, ContinueButtonLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultContinueButtonText
				: cellText;
		}

		public string GetCancelButtonText(string locale)
		{
			var dataText = DataAsset.text;
			if (string.IsNullOrEmpty(dataText))
				return DefaultCancelButtonText;

			var rowIndex = GetLocaleColumn(dataText, locale);
			var cellText = GetCellText(dataText, rowIndex, CancelButtonLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultCancelButtonText
				: cellText;
		}

		private int GetLocaleColumn(string data, string locale)
		{
			if (data == null)
				return -1;

			var lines = data.Split('\n');
			if (lines.Length == 0)
				return -1;

			var cells = ParseCsvLine(lines[0]);
			if (cells.Length == 0)
				return -1;

			for (var i = 0; i < cells.Length; i++)
			{
				if (cells[i].ToLowerInvariant() == locale)
					return i;
			}

			locale = locale.Split("_")[0];

			for (var i = 0; i < cells.Length; i++)
			{
				if (cells[i].ToLowerInvariant().StartsWith(locale))
					return i;
			}

			return -1;
		}

		private string GetCellText(string data, int rowIndex, int lineIndex)
		{
			var lines = data.Split('\n');
			if (lineIndex >= lines.Length)
				return null;

			var cells = ParseCsvLine(lines[lineIndex]);
			if (cells.Length == 0)
				return null;

			return rowIndex < cells.Length
				? cells[rowIndex].Trim('\n').Trim('\r').Trim('"')
				: null;
		}

		private static string[] ParseCsvLine(string line)
		{
			return Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
		}
	}
}
#endif