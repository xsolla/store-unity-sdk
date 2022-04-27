using System.Text;

namespace Xsolla.Core
{
    public static class UrlParameterizer
    {
		public static string GetLimitUrlParam(int? limit)
		{
			if (limit.HasValue)
				return $"&limit={limit.Value}";
			else
				return string.Empty;
		}

		public static string GetOffsetUrlParam(int? offset)
		{
			if (offset.HasValue)
				return $"&offset={offset}";
			else
				return string.Empty;
		}

		public static string GetLocaleUrlParam(string locale)
		{
			if (!string.IsNullOrEmpty(locale))
				return $"&locale={locale}";
			else
				return string.Empty;
		}

		public static string GetCurrencyUrlParam(string currency)
		{
			if (!string.IsNullOrEmpty(currency))
				return $"&currency={currency}";
			else
				return string.Empty;
		}

		public static string GetPlatformUrlParam(string platform)
		{
			if (string.IsNullOrEmpty(platform))
				return string.Empty;
			
			return $"&platform={platform}";
		}

		public static string GetAdditionalFieldsParam(string additionalFields)
		{
			if (!string.IsNullOrEmpty(additionalFields))
				return $"&additional_fields[]={additionalFields}";
			else
				return string.Empty;
		}

		public static string GetCountryUrlParam(string country)
		{
			if (!string.IsNullOrEmpty(country))
				return $"&country={country}";
			else
				return string.Empty;
		}

		public static string ConcatUrlAndParams(string url, params string[] parameters)
		{
			var isQuestionMarkRequired = !url.Contains("?");
			var isFirstParamPassed = false;
			var builder = new StringBuilder(url);

			foreach (var param in parameters)
			{
				if (string.IsNullOrEmpty(param))
					continue;

				if (isQuestionMarkRequired && !isFirstParamPassed)
				{
					isFirstParamPassed = true;
					builder.Append(param.Replace("&", "?"));
				}
				else
				{
					builder.Append(param);
				}
			}

			return builder.ToString();
		}

		public static string ConcatUrlAndParams
			(string url,
			int? limit = null,
			int? offset = null,
			string locale = null,
			string currency = null,
			string platform = null,
			string additionalFields = null,
			string country = null)
		{
			var parameters = new string[]
			{
				GetLimitUrlParam(limit),
				GetOffsetUrlParam(offset),
				GetLocaleUrlParam(locale),
				GetCurrencyUrlParam(currency),
				(!string.IsNullOrEmpty(platform) ? GetPlatformUrlParam(platform) : string.Empty),
				GetAdditionalFieldsParam(additionalFields),
				GetCountryUrlParam(country)
			};

			return ConcatUrlAndParams(url, parameters);
		}
	}
}
