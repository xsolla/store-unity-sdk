using JetBrains.Annotations;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		public Token Token { get; set; }

		private const string BASE_STORE_API_URL = "https://store.xsolla.com/api/v2/project/{0}";

		string GetLocaleUrlParam(string locale)
		{
			if (string.IsNullOrEmpty(locale))
				return string.Empty;
			else
				return $"&locale={locale}";
		}
		
		string GetCurrencyUrlParam(string currency)
		{
			if (string.IsNullOrEmpty(currency))
				return string.Empty;
			else
				return $"&currency={currency}";
		}

		string GetPlatformUrlParam()
		{
			if(XsollaSettings.Platform == PlatformType.None)
				return string.Empty;
			else
				return $"&platform={XsollaSettings.Platform.GetString()}";
		}

		private string ConcatUrlAndParams(string url, params string[] parameters)
		{
			var isQuestionMarkRequired = !url.Contains("?");
			var isFirstParamPassed = false;
			var builder = new StringBuilder(url);

			foreach (var item in parameters)
			{
				if (!string.IsNullOrEmpty(item))
				{
					if (isQuestionMarkRequired && !isFirstParamPassed)
					{
						isFirstParamPassed = true;
						builder.Append(item.Replace("&", "?"));
					}
					else
						builder.Append(item);
				}
			}

			return builder.ToString();
		}
	}
}
