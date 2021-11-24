using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		private const string ANALYTICS_URL_TEMPLATE = "engine=unity&engine_v={0}&sdk={1}&sdk_v={2}{3}";

		//Uncomment and fill the values if you want to hardcode referral info
		private KeyValuePair<string, string>? _referralAnalytics/* = new KeyValuePair<string, string>("MyReferralPlugin", "0.0.0.1")*/;

		public void SetReferralAnalytics(string referralPlugin, string referralVersion)
		{
			_referralAnalytics = new KeyValuePair<string, string>(referralPlugin, referralVersion);
		}

		public string AppendAnalyticsToUrl(SdkType analyticsType, string url)
		{
			if (analyticsType == SdkType.None)
			{
				Debug.LogWarning("Attempt to add analytics without providing analyticsType parameter");
				return url;
			}

			var dividingSymbol = url.Contains("?") ? "&" : "?";
			var engineVersion = Application.unityVersion;

			string sdkType;
			string sdkVersion;
			GetSdkSpecificParameters(analyticsType, out sdkType, out sdkVersion);

			var referralAnalytics = default(string);

			if (_referralAnalytics.HasValue)//if (_referralAnalytics != null)
			{
				string referralPlugin = _referralAnalytics.Value.Key;
				string referralVersion = _referralAnalytics.Value.Value;
				referralAnalytics = string.Format("&ref={0}&ref_v={1}", referralPlugin, referralVersion);
			}

			var analyticsAddition = string.Format(ANALYTICS_URL_TEMPLATE, engineVersion, sdkType, sdkVersion, referralAnalytics);

			var result =  string.Format("{0}{1}{2}", url, dividingSymbol, analyticsAddition);
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

			string sdkType;
			string sdkVersion;
			GetSdkSpecificParameters(analyticsType, out sdkType, out sdkVersion);

			var resultCapacity = _referralAnalytics.HasValue ? 6 : 4;
			var result = new List<WebRequestHeader>(capacity: resultCapacity)
			{
				new WebRequestHeader() { Name = "X-ENGINE", Value = "UNITY" },
				new WebRequestHeader() { Name = "X-ENGINE-V", Value = Application.unityVersion.ToUpper() },
				new WebRequestHeader() { Name = "X-SDK", Value = sdkType.ToUpper() },
				new WebRequestHeader() { Name = "X-SDK-V", Value = sdkVersion.ToUpper() }
			};

			if (_referralAnalytics.HasValue)//if (_referralAnalytics != null)
			{
				string referralPlugin = _referralAnalytics.Value.Key;
				string referralVersion = _referralAnalytics.Value.Value;

				result.Add( new WebRequestHeader() { Name = "X-REF", Value = referralPlugin.ToUpper() } );
				result.Add( new WebRequestHeader() { Name = "X-REF-V", Value = referralVersion.ToUpper() });
			}

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
					Debug.LogError(string.Format("Unexpected analyticsType: '{0}'", analyticsType.ToString()));
					break;
			}
		}
	}
}
