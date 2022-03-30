using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

using Debug = UnityEngine.Debug;

namespace Xsolla.Tests
{
    public class XsollaAuthSocialTests
    {
		[Test]
		public void GetSocialNetworkAuthUrl_JWT_Success()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.JWT;
			var url = XsollaAuth.Instance.GetSocialNetworkAuthUrl(providerName: SocialProvider.Facebook);
			Debug.Log($"AUTH URL: {url}");
			Assert.IsTrue(!string.IsNullOrEmpty(url));
		}

		[Test]
		public void GetSocialNetworkAuthUrl_OAuth_Success()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;
			var url = XsollaAuth.Instance.GetSocialNetworkAuthUrl(providerName: SocialProvider.Facebook);
			Debug.Log($"AUTH URL: {url}");
			Assert.IsTrue(!string.IsNullOrEmpty(url));
		}

		private IEnumerator GetLinksForSocialAuth(string testName, string locale)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<List<SocialNetworkLink>> onSuccess = links =>
			{
				if (links != null)
					success = true;
				else
				{
					success = false;
					errorMessage = "LINKS ARE NULL";
				}
			};
			
			Action<Error> onError = error =>
			{
				success = false;
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
			};

			XsollaAuth.Instance.GetLinksForSocialAuth(locale, onSuccess, onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(testName, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_NoLocale_Success()
		{
			yield return GetLinksForSocialAuth(nameof(GetLinksForSocialAuth_NoLocale_Success), null);
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_enUS_Success()
		{
			yield return GetLinksForSocialAuth(nameof(GetLinksForSocialAuth_enUS_Success), "en_US");
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_ruRU_Success()
		{
			yield return GetLinksForSocialAuth(nameof(GetLinksForSocialAuth_ruRU_Success), "ru_RU");
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetLinksForSocialAuth(nameof(GetLinksForSocialAuth_InvalidToken_SuccessAndTokenRefreshed), null);
			TestHelper.CheckTokenChanged(nameof(GetLinksForSocialAuth_InvalidToken_SuccessAndTokenRefreshed));
		}
	}
}
