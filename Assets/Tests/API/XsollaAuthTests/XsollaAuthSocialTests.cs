using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaAuthSocialTests
	{
		[Test]
		public void GetSocialNetworkAuthUrl_JWT_Success()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.JWT;
			GetSocialNetworkAuthUrl();
		}

		[Test]
		public void GetSocialNetworkAuthUrl_OAuth_Success()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;
			GetSocialNetworkAuthUrl();
		}

		private void GetSocialNetworkAuthUrl([CallerMemberName] string testName = null)
		{
			var url = XsollaAuth.Instance.GetSocialNetworkAuthUrl(providerName: SocialProvider.Facebook);
			if (!string.IsNullOrEmpty(url))
				TestHelper.Pass($"auth URL: {url}",testName);
			else
				TestHelper.Fail("auth URL is NULL",testName);
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_NoLocale_Success()
		{
			yield return GetLinksForSocialAuth(locale: null);
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_deDE_Locale_Success()
		{
			yield return GetLinksForSocialAuth(locale: "de_DE");
		}

		[UnityTest]
		public IEnumerator GetLinksForSocialAuth_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetLinksForSocialAuth(locale: null);
			TestHelper.CheckTokenChanged();
		}

		private IEnumerator GetLinksForSocialAuth([CallerMemberName] string testName = null, string locale = null)
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
				TestHelper.Fail(errorMessage, testName);
		}
	}
}