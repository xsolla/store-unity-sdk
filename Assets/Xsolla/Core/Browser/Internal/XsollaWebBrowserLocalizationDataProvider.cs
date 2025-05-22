#if UNITY_WEBGL
using System.Text.RegularExpressions;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class XsollaWebBrowserLocalizationDataProvider
	{
		private static TextAsset _dataAsset;

		private static TextAsset DataAsset
		{
			get
			{
				if (!_dataAsset)
					_dataAsset = Resources.Load<TextAsset>("SafariPopupLocalization");

				return _dataAsset;
			}
		}

		private const string DefaultMessage = "Open a payment page in a new tab?";
		private const string DefaultContinueButtonText = "Yes, open";
		private const string DefaultCancelButtonText = "Cancel";

		private const int MessageLineIndex = 1;
		private const int ContinueButtonLineIndex = 3;
		private const int CancelButtonLineIndex = 4;

		public static string GetMessageText(string locale)
		{
			var dataText = DataAsset.text;
			if (string.IsNullOrEmpty(dataText))
				return DefaultMessage;

			var rowIndex = GetRowIndex(dataText, locale);
			var cellText = GetCellText(dataText, rowIndex, MessageLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultMessage
				: cellText;
		}

		public static string GetContinueButtonText(string locale)
		{
			var dataText = DataAsset.text;
			if (string.IsNullOrEmpty(dataText))
				return DefaultContinueButtonText;

			var rowIndex = GetRowIndex(dataText, locale);
			var cellText = GetCellText(dataText, rowIndex, ContinueButtonLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultContinueButtonText
				: cellText;
		}

		public static string GetCancelButtonText(string locale)
		{
			var dataText = DataAsset.text;
			if (string.IsNullOrEmpty(dataText))
				return DefaultCancelButtonText;

			var rowIndex = GetRowIndex(dataText, locale);
			var cellText = GetCellText(dataText, rowIndex, CancelButtonLineIndex);

			return string.IsNullOrEmpty(cellText)
				? DefaultCancelButtonText
				: cellText;
		}

		private static int GetRowIndex(string data, string locale)
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

		private static string GetCellText(string data, int rowIndex, int lineIndex)
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