using System.Runtime.CompilerServices;
using NUnit.Framework;
using Xsolla.Core;

using Debug = UnityEngine.Debug;

namespace Xsolla.Tests
{
	public class TestHelper
	{
		public static void Pass([CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}'");
		}

		public static void Pass(string additionalInfo, [CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}' info: '{additionalInfo}'");
		}

		public static void Pass(TestSignInHelper.SignInResult signInResult, [CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}' signInSuccess: '{signInResult?.success}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'");
		}

		public static void Fail([CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(string additionalInfo, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' info: '{additionalInfo}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(TestSignInHelper.SignInResult signInResult, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' signInSuccess: '{signInResult?.success}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(Error error, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' error: '{error?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void CheckTokenChanged([CallerMemberName] string testName = null)
		{
			string oldToken = TestSignInHelper.Instance.OldToken;
			CheckTokenChanged(oldToken, testName);
		}

		public static void CheckTokenChanged(string oldToken, [CallerMemberName] string testName = null)
		{
			string newToken = Token.Instance;

			if (newToken != oldToken)
				Pass("TOKEN CHANGED", testName);
			else
				Fail("TOKEN DID NOT CHANGE", testName);
		}
	}
}