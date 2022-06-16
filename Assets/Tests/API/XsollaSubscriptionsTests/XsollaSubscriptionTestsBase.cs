namespace Xsolla.Tests
{
	public class XsollaSubscriptionTestsBase
	{
		protected const string TEST_PROJECT_ID = "149697";

		protected bool AreEqual<T>(string parameterName, T expected, T actual, ref string errorMessage)
		{
			if (expected.Equals(actual))
				return true;

			errorMessage = $"{parameterName.ToUpperInvariant()} EXPECTED: \"{expected.ToString().ToUpperInvariant()}\" BUT WAS: \"{actual.ToString().ToUpperInvariant()}\"";
			return false;
		}

		protected bool IsNull<T>(string parameterName, T actual, ref string errorMessage)
		{
			if (actual == null)
				return true;

			errorMessage = $"{parameterName.ToUpperInvariant()} EXPECTED: NULL BUT WAS: NOT NULL";
			return false;
		}

		protected bool NotNull<T>(string parameterName, T actual, ref string errorMessage)
		{
			if (actual != null)
				return true;

			errorMessage = $"{parameterName.ToUpperInvariant()} EXPECTED: NOT NULL BUT WAS: NULL";
			return false;
		}

		protected bool NotNull(string parameterName, string actual, ref string errorMessage)
		{
			if (!string.IsNullOrEmpty(actual))
				return true;

			errorMessage = $"{parameterName.ToUpperInvariant()} EXPECTED: NOT NULL BUT WAS: NULL";
			return false;
		}

		protected void HandleResult(string testName, bool? success, string errorMessage)
		{
			if (success.HasValue && success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(testName, errorMessage);
		}
	}
}