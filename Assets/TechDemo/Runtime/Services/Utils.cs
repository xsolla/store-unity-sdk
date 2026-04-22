using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.TechDemo
{
	public static class Utils
	{
		public static void DeleteAllChildren(this Transform parent)
		{
			if (!parent)
				throw new ArgumentNullException(nameof(parent));

			var childCount = parent.childCount;
			for (var i = childCount - 1; i >= 0; i--)
			{
				var child = parent.GetChild(i);
				if (child)
				{
					child.gameObject.SetActive(false);
					Object.Destroy(child.gameObject);
				}
			}
		}

		public static string Nicify(this string value)
		{
			string returnValue;
			returnValue = string.Concat(value, "");

			//Strip leading "_" character
			returnValue = Regex.Replace(returnValue, "^_", "").Trim();

			//Add a space between each lower case character and upper case character
			returnValue = Regex.Replace(returnValue, "([a-z])([A-Z])", "$1 $2").Trim();

			//Add a space between 2 upper case characters when the second one is followed by a lower space character
			returnValue = Regex.Replace(returnValue, "([A-Z])([A-Z][a-z])", "$1 $2").Trim();

			return returnValue;
		}

		public static string Slugify(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;

			// Удаляем пробелы между словами и заменяем их на нижнее подчеркивание
			var underscored = Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1_$2");

			// Приводим к нижнему регистру
			return underscored.ToLowerInvariant();
		}
	}
}