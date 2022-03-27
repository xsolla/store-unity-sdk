using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Auth;

namespace Xsolla.Tests
{
	public class XsollaAuthOAuthRefreshTokenTests
    {
		[OneTimeSetUp]
		public void OneTimeSetUp() => TestSignInHelper.Instance.Setup();

		[OneTimeTearDown]
		public void OneTimeTearDown() => TestSignInHelper.Instance.TearDown();

		[UnityTest]
        public IEnumerator RefreshOAuthToken_TokenChanged()
        {
			yield return TestSignInHelper.Instance.CheckSession();

			string prevToken = Token.Instance;
			bool? success = default;
			string errorMessage = default;

			XsollaAuth.Instance.RefreshOAuthToken(
				onSuccess: _ =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (!success.Value)
				TestHelper.Fail(nameof(RefreshOAuthToken_TokenChanged), errorMessage);
			else
				TestHelper.CheckTokenChanged(prevToken, nameof(RefreshOAuthToken_TokenChanged));
		}

		[UnityTest]
		public IEnumerator RefreshOAuthToken_OldRefreshToken_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;
			var errorType = ErrorType.Undefined;

			var oldToken = "W0qQwxh5rJe4wK3VREv1qsGnxUKQfZNvJkYJq7lMPCA.6HeK7wKy1cZAy2STRk8sTcplcA7TiglrT5K3kn-lClc";

			XsollaAuth.Instance.RefreshOAuthToken(
				refreshToken: oldToken,
				onSuccess: _ =>
				{
					success = true;
				},
				onError: error =>
				{
					errorType = error?.ErrorType ?? ErrorType.Undefined;
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Fail(nameof(RefreshOAuthToken_OldRefreshToken_Failure), "Expected call to fail");
			else if (errorType != ErrorType.InvalidToken)
				TestHelper.Fail(nameof(RefreshOAuthToken_OldRefreshToken_Failure), $"Unexpected error type: {errorType}");
			else
				TestHelper.Pass(nameof(RefreshOAuthToken_OldRefreshToken_Failure), errorMessage);
		}
	}
}
