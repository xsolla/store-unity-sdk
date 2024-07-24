using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Xsolla.Core;

namespace Xsolla.Tests.WebRequests
{
	public class WebRequestTestBase
	{
		protected static string CreateUrlWithAnalytics(SdkType? sdkType = null)
		{
			var sdkTypeValue = sdkType ?? SdkType.Login;
			return WebRequestHelper.Instance.AppendAnalyticsToUrl(sdkTypeValue, "https://domain.com/api");
		}

		protected static NameValueCollection ParseQueryParams(string url)
		{
			url = url.Replace("?", "&");
			var collection = HttpUtility.ParseQueryString(url);
			collection.Remove(null);
			return collection;
		}

		protected static List<WebRequestHeader> GetWebRequestHeadersCollection(SdkType? sdkType = null)
		{
			var sdkTypeValue = sdkType ?? SdkType.Login;
			return WebRequestHelper.Instance.GetAnalyticHeaders(sdkTypeValue);
		}
	}
}