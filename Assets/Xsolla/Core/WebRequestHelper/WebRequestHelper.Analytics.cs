using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		private const string ANALYTICS_URL_TEMPLATE = "engine=unity&engine_v={0}&sdk={1}&sdk_v={2}&build_platform={3}{4}";

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
			GetUnityParameters(toUpper: false, out string engineVersion, out string buildPlatform);
			GetXsollaSdkParameters(analyticsType, toUpper: false, out string sdkType, out string sdkVersion);

			var referralAnalytics = default(string);

			if (_referralAnalytics.HasValue)//if (_referralAnalytics != null)
			{
				string referralPlugin = _referralAnalytics.Value.Key;
				string referralVersion = _referralAnalytics.Value.Value;
				referralAnalytics = $"&ref={referralPlugin}&ref_v={referralVersion}";
			}

			var analyticsAddition = string.Format(ANALYTICS_URL_TEMPLATE, engineVersion, sdkType, sdkVersion, buildPlatform, referralAnalytics);

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

			GetUnityParameters(toUpper: true, out string engineVersion, out string buildPlatform);
			GetXsollaSdkParameters(analyticsType, toUpper: true, out string sdkType, out string sdkVersion);

			var resultCapacity = _referralAnalytics.HasValue ? 6 : 4;
			var result = new List<WebRequestHeader>(capacity: resultCapacity)
			{
				new WebRequestHeader() { Name = "X-ENGINE", Value = "UNITY" },
				new WebRequestHeader() { Name = "X-ENGINE-V", Value = engineVersion },
				new WebRequestHeader() { Name = "X-SDK", Value = sdkType },
				new WebRequestHeader() { Name = "X-SDK-V", Value = sdkVersion },
				new WebRequestHeader() { Name = "X-BUILD-PLATFORM", Value = buildPlatform }
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

		private void GetXsollaSdkParameters(SdkType analyticsType, bool toUpper, out string sdkType, out string sdkVersion)
		{
			switch (analyticsType)
			{
				case SdkType.Login:
					sdkType = toUpper ? "LOGIN" : "login";
					sdkVersion = Constants.LoginSdkVersion;
					break;
				case SdkType.Store:
					sdkType = toUpper ? "STORE" : "store";
					sdkVersion = Constants.StoreSdkVersion;
					break;
				case SdkType.Subscriptions:
					sdkType = toUpper ? "SUBSCRIPTIONS" : "subscriptions";
					sdkVersion = Constants.StoreSdkVersion;
					break;
				default:
					Debug.LogError($"Unexpected analyticsType: '{analyticsType}'");
					sdkType = string.Empty;
					sdkVersion = string.Empty;
					break;
			}
		}

		private void GetUnityParameters(bool toUpper, out string engineVersion, out string buildPlatform)
		{
			engineVersion = Application.unityVersion;
#if UNITY_EDITOR
			buildPlatform = EditorUserBuildSettings.activeBuildTarget.ToString();
#else
			buildPlatform = Application.platform.ToString();
#endif

			if (toUpper)
			{
				engineVersion = engineVersion.ToUpper();
				buildPlatform = buildPlatform.ToUpper();
			}
			else
				buildPlatform = buildPlatform.ToLower();
		}
	}
}