using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Auth;
using NUnit.Framework;

namespace Xsolla.Tests
{
	public class XsollaAuthOAuthRefreshTokenTests
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear() => TestHelper.Clear();

		[UnityTest]
		public IEnumerator RefreshOAuthToken_TokenChanged()
		{
			yield return TestSignInHelper.Instance.GenerateNewSession();

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

			if (success.Value)
				TestHelper.CheckTokenChanged(oldToken: prevToken);
			else
				TestHelper.Fail(errorMessage);
		}

		[UnityTest]
		public IEnumerator RefreshOAuthToken_OldRefreshToken_Failure()
		{
			yield return TestSignInHelper.Instance.GenerateNewSession();

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
				TestHelper.Fail(additionalInfo: "Expected call to fail");
			else if (errorType != ErrorType.InvalidToken)
				TestHelper.Fail(additionalInfo: $"Unexpected error type: {errorType}");
			else
				TestHelper.Pass(additionalInfo: errorMessage);
		}
	}
}