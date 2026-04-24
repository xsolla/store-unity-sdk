using System.Collections;
using NUnit.Framework;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class TestBase
	{
		protected static void ClearEnv()
		{
			DeleteSavedToken();
			Object.DestroyImmediate(WebRequestHelper.Instance.gameObject);
			XsollaSettings.StoreProjectId = "77640";
		}

		protected static void DeleteSavedToken()
		{
			XsollaToken.DeleteSavedInstance();
		}

		protected static IEnumerator SetOldAccessToken()
		{
			yield return SignIn();
			var refreshToken = XsollaToken.RefreshToken;
			XsollaToken.Create(TestCredentials.OldAccessToken, refreshToken);
		}

		protected static IEnumerator SetOldRefreshToken()
		{
			yield return SignIn();
			var accessToken = XsollaToken.AccessToken;
			XsollaToken.Create(accessToken, TestCredentials.OldRefreshToken);
		}

		protected static IEnumerator CheckSession()
		{
			if (!XsollaToken.Exists)
				yield return SignIn();
		}

		protected static IEnumerator SignInAsTestUser()
		{
			yield return SignIn(TestCredentials.SdkUsername, TestCredentials.SdkPassword);
		}

		protected static IEnumerator SignIn()
		{
			yield return SignIn(TestCredentials.DefaultUsername, TestCredentials.DefaultPassword);
		}

		private static IEnumerator SignIn(string username, string password)
		{
			var isComplete = false;
			XsollaAuth.SignIn(
				username,
				password,
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		protected static IEnumerator SignOut()
		{
			if (!XsollaToken.Exists)
				yield break;

			var isComplete = false;
			XsollaAuth.Logout(
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}