using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		private const string ANALYTICS_URL_TEMPLATE = "engine=unity&engine_v={0}&sdk={1}&sdk_v={2}";

		public string AppendAnalyticsToUrl(SdkType analyticsType, string url)
		{
			if (analyticsType == SdkType.None)
			{
				Debug.LogWarning("Attempt to add analytics without providing analyticsType parameter");
				return url;
			}

			var dividingSymbol = url.Contains("?") ? "&" : "?";
			var engineVersion = Application.unityVersion;

			GetSdkSpecificParameters(analyticsType, out string sdkType, out string sdkVersion);

			var analyticsAddition = string.Format(ANALYTICS_URL_TEMPLATE, engineVersion, sdkType, sdkVersion);

			var result =  $"{url}{dividingSymbol}{analyticsAddition}";
			return result;
		}

		private List<WebRequestHeader> AppendAnalyticHeaders(SdkType analyticsType, params WebRequestHeader[] headers)
		{
			if (analyticsType == SdkType.None)
			{
				Debug.LogWarning("Attempt to add analytics without providing analyticsType parameter");
				if (headers != null)
					return new List<WebRequestHeader>(headers);
				else
					return null;
			}

			var analyticHeaders = GetAnalyticHeaders(analyticsType);
			var result = default(List<WebRequestHeader>);

			if (headers != null)
			{
				result = new List<WebRequestHeader>(headers);
				result.AddRange(analyticHeaders);
			}
			else
			{
				Debug.LogWarning("Attmpt to append analytic headers to 'null'");
				result = analyticHeaders;
			}

			return result;
		}

		private List<WebRequestHeader> GetAnalyticHeaders(SdkType analyticsType)
		{
			//These headers cause network error in WebGL environment
			//'Access to XMLHttpRequest ... has been blocked by CORS policy: Request header field x-engine is not allowed by Access-Control-Allow-Headers in preflight response.'
			if (Application.platform == RuntimePlatform.WebGLPlayer)
				return new List<WebRequestHeader>();

			GetSdkSpecificParameters(analyticsType, out string sdkType, out string sdkVersion);

			var result = new List<WebRequestHeader>
			{
				new WebRequestHeader() { Name = "X-ENGINE", Value = "UNITY" },
				new WebRequestHeader() { Name = "X-ENGINE-V", Value = Application.unityVersion.ToUpper() },
				new WebRequestHeader() { Name = "X-SDK", Value = sdkType.ToUpper() },
				new WebRequestHeader() { Name = "X-SDK-V", Value = sdkVersion.ToUpper() },
			};

			return result;
		}

		private void GetSdkSpecificParameters(SdkType analyticsType, out string sdkType, out string sdkVersion)
		{
			sdkType = default(string);
			sdkVersion = default(string);

			switch (analyticsType)
			{
				case SdkType.Login:
					sdkType = "login";
					sdkVersion = Constants.LoginSdkVersion;
					break;
				case SdkType.Store:
					sdkType = "store";
					sdkVersion = Constants.StoreSdkVersion;
					break;
				default:
					Debug.LogError($"Unexpected analyticsType: '{analyticsType.ToString()}'");
					break;
			}
		}
	}
}
