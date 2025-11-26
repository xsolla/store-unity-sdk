using System.Text.RegularExpressions;
using UnityEngine;

namespace Xsolla.Core
{
	internal class CsvLocalizationParser
	{
		private readonly string Content;

		public CsvLocalizationParser(TextAsset asset)
		{
			if (asset)
				Content = asset.text;
			else
				Debug.LogError("CsvReader: TextAsset is null");
		}

		public string GetCellTextOrFallback(string locale, int row, string fallback)
		{
			if (string.IsNullOrEmpty(locale))
				return fallback;

			if (string.IsNullOrEmpty(Content))
				return fallback;

			var column = GetLocaleColumn(Content, locale);
			var cellText = GetCellText(Content, column, row);

			return string.IsNullOrEmpty(cellText)
				? fallback
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