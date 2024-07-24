using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

namespace Xsolla.Tests.WebRequests
{
	public class AnalyticsParametersCultureTests : WebRequestTestBase
	{
		private CultureInfo originalCulture;
		private CultureInfo originalUICulture;

		[SetUp]
		public void SetUp()
		{
			originalCulture = Thread.CurrentThread.CurrentCulture;
			originalUICulture = Thread.CurrentThread.CurrentUICulture;
		}

		[TearDown]
		public void TearDown()
		{
			Thread.CurrentThread.CurrentCulture = originalCulture;
			Thread.CurrentThread.CurrentUICulture = originalUICulture;
		}

		[Test]
		[TestCase("en-US")]
		[TestCase("es-ES")]
		[TestCase("tr-TR")]
		[TestCase("ru-RU")]
		[TestCase("zh-CN")]
		[TestCase("ko-KR")]
		[TestCase("ar-SA")]
		public void UrlInvariantCultureValidation(string cultureName)
		{
			SetCulture(cultureName);

			var url = CreateUrlWithAnalytics();
			var queryParams = ParseQueryParams(url);
			foreach (string key in queryParams)
			{
				CheckContainsOnlyInvariantCharacters(key);
				CheckContainsOnlyInvariantCharacters(queryParams.Get(key));
			}
		}

		[Test]
		[TestCase("en-US")]
		[TestCase("es-ES")]
		[TestCase("tr-TR")]
		[TestCase("ru-RU")]
		[TestCase("zh-CN")]
		[TestCase("ko-KR")]
		[TestCase("ar-SA")]
		public void HeadersInvariantCultureValidation(string cultureName)
		{
			SetCulture(cultureName);

			var headers = GetWebRequestHeadersCollection();
			foreach (var header in headers)
			{
				CheckContainsOnlyInvariantCharacters(header.Name);
				CheckContainsOnlyInvariantCharacters(header.Value);
			}
		}

		private static void CheckContainsOnlyInvariantCharacters(string param)
		{
			var invariantRegex = new Regex("^[a-zA-Z0-9._-]*$");
			var isInvariant = invariantRegex.IsMatch(param);
			Assert.IsTrue(isInvariant, $"Query param '{param}' should contain only invariant characters. Param: {param}");
		}

		private static void SetCulture(string cultureName)
		{
			var newCulture = new CultureInfo(cultureName);
			Thread.CurrentThread.CurrentCulture = newCulture;
			Thread.CurrentThread.CurrentUICulture = newCulture;
		}
	}
}