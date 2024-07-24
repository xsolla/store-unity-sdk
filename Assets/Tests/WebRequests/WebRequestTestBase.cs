using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
			var queryParameters = DecodeQueryParameters(new Uri(url));

			var collection = new NameValueCollection();
			foreach (var kvp in queryParameters)
			{
					collection.Add(kvp.Key, kvp.Value);
			}

			collection.Remove(null);
			return collection;
		}

		private static Dictionary<string, string> DecodeQueryParameters(Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			if (uri.Query.Length == 0)
				return new Dictionary<string, string>();

			return uri.Query.TrimStart('?')
				.Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
				.GroupBy(parts => parts[0],
					parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
				.ToDictionary(grouping => grouping.Key,
					grouping => string.Join(",", grouping));
		}

		protected static List<WebRequestHeader> GetWebRequestHeadersCollection(SdkType? sdkType = null)
		{
			var sdkTypeValue = sdkType ?? SdkType.Login;
			return WebRequestHelper.Instance.GetAnalyticHeaders(sdkTypeValue);
		}
	}
}