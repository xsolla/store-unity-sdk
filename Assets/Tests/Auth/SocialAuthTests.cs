using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests.Auth
{
	public class SocialAuthTests : AuthTestsBase
	{
		[Test]
		public void GetSocialNetworkAuthUrl_Success()
		{
			var url = XsollaAuth.GetSocialNetworkAuthUrl(SocialProvider.Facebook);
			Assert.IsFalse(string.IsNullOrEmpty(url));
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_NoLocale_Success()
		{
			yield return GetLinksForSocialAuth();
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_deDE_Locale_Success()
		{
			yield return GetLinksForSocialAuth("de_DE");
		}

		// [UnityTest]
		// public IEnumerator GetLinksForSocialAuth_InvalidToken_SuccessAndTokenRefreshed()
		// {
		// 	yield return SetOldAccessToken();
		// 	var oldToken = XsollaToken.AccessToken;
		// 	yield return GetLinksForSocialAuth();
		// 	Assert.AreNotEqual(oldToken, XsollaToken.AccessToken);
		// }

		private static IEnumerator GetLinksForSocialAuth(string locale = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaAuth.GetLinksForSocialAuth(
				links =>
				{
					isComplete = true;
					Assert.NotNull(links);
					Assert.NotNull(links.items);
					Assert.Greater(links.items.Count, 0);
					Assert.IsFalse(string.IsNullOrEmpty(links.items[0].provider));
					Assert.IsFalse(string.IsNullOrEmpty(links.items[0].auth_url));
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				locale);

			yield return new WaitUntil(() => isComplete);
		}
	}
}