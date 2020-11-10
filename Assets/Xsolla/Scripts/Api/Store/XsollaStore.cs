using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		public Token Token { get; set; }

		private const string BASE_STORE_API_URL = "https://store.xsolla.com/api/v2/project/{0}";

		string AnalyticUrlAddition => $"?engine=unity&engine_v={Application.unityVersion}&sdk=store&sdk_v={Constants.StoreSdkVersion}";

		private List<WebRequestHeader> AnalyticHeaders
		{
			get
			{
				if (Application.platform != RuntimePlatform.WebGLPlayer)
				{
					return new List<WebRequestHeader>
					{
						new WebRequestHeader() { Name = "X-ENGINE", Value = "UNITY" },
						new WebRequestHeader() { Name = "X-ENGINE-V", Value = Application.unityVersion.ToUpper() },
						new WebRequestHeader() { Name = "X-SDK", Value = "STORE" },
						new WebRequestHeader() { Name = "X-SDK-V", Value = Constants.StoreSdkVersion.ToUpper() },
					};
				}
				else
					return new List<WebRequestHeader>();
			}
		}

		private List<WebRequestHeader> AppendAnalyticHeadersTo(params WebRequestHeader[] headers)
		{
			var analyticHeaders = AnalyticHeaders;
			var result = new List<WebRequestHeader>(capacity: analyticHeaders.Count + headers.Length);

			result.AddRange(headers);
			result.AddRange(analyticHeaders);

			return result;
		}

		private List<WebRequestHeader> AuthAndAnalyticHeaders => AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(Token));

		string GetLocaleUrlParam(string locale)
		{
			if (string.IsNullOrEmpty(locale))
			{
				return string.Empty;
			}
			return string.Format("&locale={0}", locale);
		}
		
		string GetCurrencyUrlParam(string currency)
		{
			if (string.IsNullOrEmpty(currency))
			{
				return string.Empty;
			}
			return string.Format("&currency={0}", currency);
		}

		string GetPlatformUrlParam()
		{
			if(XsollaSettings.Platform == PlatformType.None) {
				return string.Empty;
			}
			return "&platform=" + XsollaSettings.Platform.GetString();
		}
	}
}