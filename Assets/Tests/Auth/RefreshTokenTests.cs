using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests.Auth
{
	public class RefreshTokenTests : AuthTestsBase
	{
		[UnityTest]
		public IEnumerator RefreshToken_Success()
		{
			yield return SignIn();
			var prevToken = XsollaToken.AccessToken;

			var isComplete = false;
			XsollaAuth.RefreshToken(
				() =>
				{
					isComplete = true;
					Assert.AreNotEqual(prevToken, XsollaToken.AccessToken);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator RefreshToken_OldRefreshToken_Failure()
		{
			yield return SignIn();
			yield return SetOldRefreshToken();

			var isComplete = false;
			XsollaAuth.RefreshToken(
				() =>
				{
					isComplete = true;
					Assert.Fail("Refresh token is old");
				},
				error =>
				{
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
					Assert.AreEqual(error.ErrorType, ErrorType.InvalidToken);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}