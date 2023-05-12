using System.Collections;
using NUnit.Framework;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class TestBase
	{
		private const string OLD_ACCESS_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOltdLCJlbWFpbCI6Inhzb2xsYXN0b3Jlc2RrLmFAZ21haWwuY29tIiwiZXhwIjoxNjQ4NDYwMzc2LCJncm91cHMiOlt7ImlkIjo2NDgxLCJuYW1lIjoiZGVmYXVsdCIsImlzX2RlZmF1bHQiOnRydWV9XSwiaWF0IjoxNjQ4MzczOTc2LCJpc19tYXN0ZXIiOnRydWUsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImp0aSI6ImFhNzRhMTVhLWIxNWQtNGM4MS1hYzUxLTdmNGE1OTVkYWU5YyIsInByb21vX2VtYWlsX2FncmVlbWVudCI6dHJ1ZSwicHVibGlzaGVyX2lkIjoxMzY1OTMsInNjcCI6WyJvZmZsaW5lIl0sInN1YiI6IjE0ZGNiMzQzLWFkMjUtNDZhOS05YjE2LWJjZWVjNWNmN2NjZiIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ1c2VybmFtZSI6Inhzb2xsYSIsInhzb2xsYV9sb2dpbl9hY2Nlc3Nfa2V5Ijoialpta3dObUlsZ25IZVVCSE8xRURRUFhleHJhdjJYdEk4Y1FJeF90ajliZyIsInhzb2xsYV9sb2dpbl9wcm9qZWN0X2lkIjoiMDI2MjAxZTMtN2U0MC0xMWVhLWE4NWItNDIwMTBhYTgwMDA0In0.Y4qJ0_UStkND4Xsjhy1P0EUv1DLswxybxLKW8IV6N3Y";
		private const string OLD_REFRESH_TOKEN = "W0qQwxh5rJe4wK3VREv1qsGnxUKQfZNvJkYJq7lMPCA.6HeK7wKy1cZAy2STRk8sTcplcA7TiglrT5K3kn-lClc";

		protected static void ClearEnv()
		{
			DeleteSavedToken();
			Object.DestroyImmediate(WebRequestHelper.Instance.gameObject);
		}

		protected static void DeleteSavedToken()
		{
			XsollaToken.DeleteSavedInstance();
		}

		protected static IEnumerator SetOldAccessToken()
		{
			yield return SignIn();
			var refreshToken = XsollaToken.RefreshToken;
			XsollaToken.Create(OLD_ACCESS_TOKEN, refreshToken);
		}

		protected static IEnumerator SetOldRefreshToken()
		{
			yield return SignIn();
			var accessToken = XsollaToken.AccessToken;
			XsollaToken.Create(accessToken, OLD_REFRESH_TOKEN);
		}

		protected static IEnumerator CheckSession()
		{
			if (!XsollaToken.Exists)
				yield return SignIn();
		}

		protected static IEnumerator SignInAsTestUser()
		{
			yield return SignIn("sdk@xsolla.com", "1qazXSW@");
		}

		protected static IEnumerator SignIn()
		{
			yield return SignIn("xsolla", "xsolla");
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