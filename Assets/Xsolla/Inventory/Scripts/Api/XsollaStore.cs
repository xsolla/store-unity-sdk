using JetBrains.Annotations;
using System.Text;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string BASE_STORE_API_URL = "https://store.xsolla.com/api/v2/project/{0}";

		string GetLimitUrlParam(int? limit)
		{
			if (limit == null)
				return string.Empty;
			else
				return string.Format("&limit={0}", limit);
		}

		string GetOffsetUrlParam(int? offset)
		{
			if (offset == null)
				return string.Empty;
			else
				return string.Format("&offset={0}", offset);
		}

		string GetLocaleUrlParam(string locale)
		{
			if (string.IsNullOrEmpty(locale))
				return string.Empty;
			else
				return string.Format("&locale={0}", locale);
		}
		
		string GetCurrencyUrlParam(string currency)
		{
			if (string.IsNullOrEmpty(currency))
				return string.Empty;
			else
				return string.Format("&currency={0}", currency);
		}

		string GetPlatformUrlParam()
		{
			if(XsollaSettings.Platform == PlatformType.None)
				return string.Empty;
			else
				return string.Format("&platform={0}", XsollaSettings.Platform.GetString());
		}

		string GetAdditionalFieldsParam(string additionalFields)
		{
			if (string.IsNullOrEmpty(additionalFields))
				return string.Empty;
			else
				return string.Format("&additional_fields[]={0}", additionalFields);
		}

		string GetCountryUrlParam(string country)
		{
			if (string.IsNullOrEmpty(country))
				return string.Empty;
			else
				return string.Format("&country={0}", country);
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
