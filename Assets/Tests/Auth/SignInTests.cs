using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests.Auth
{
	public class SignInTests : AuthTestsBase
	{
		[UnityTest]
		public IEnumerator SignIn_Success()
		{
			var isComplete = false;
			XsollaAuth.SignIn(
				"xsolla",
				"xsolla",
				() => {
					isComplete = true;
					Assert.NotNull(XsollaToken.AccessToken);
					Assert.NotNull(XsollaToken.RefreshToken);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator SignIn_IncorrectData_Failure()
		{
			var isComplete = false;
			XsollaAuth.SignIn(
				"k4TCNgHs",
				"k4TCNgHs",
				() => {
					isComplete = true;
					Assert.Fail("Incorrect username and password should not be accepted");
				},
				error => {
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetUserInfo_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaAuth.GetUserInfo(
				userInfo => {
					isComplete = true;
					Assert.NotNull(userInfo);
					Assert.NotNull(userInfo.id);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator Logout_Sso_Success()
		{
			yield return Logout(LogoutType.Sso);
		}

		[UnityTest]
		public IEnumerator Logout_All_Success()
		{
			yield return Logout(LogoutType.All);
		}

		private static IEnumerator Logout(LogoutType logoutType)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaAuth.Logout(
				() => isComplete = true,
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				logoutType);

			yield return new WaitUntil(() => isComplete);
		}
	}
}