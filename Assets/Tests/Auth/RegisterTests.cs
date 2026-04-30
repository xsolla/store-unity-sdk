using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;

namespace Xsolla.Tests.Auth
{
	public class RegisterTests : AuthTestsBase
	{
		[UnityTest]
		public IEnumerator Register_Success()
		{
			var username = TestUtils.GenerateRandomString();
			var password = TestUtils.GenerateRandomString();
			var email = TestUtils.GenerateRandomEmail();

			var isComplete = false;
			XsollaAuth.Register(
				username,
				password,
				email,
				_ => isComplete = true,
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator ResendConfirmationLink_Success()
		{
			var username = TestUtils.GenerateRandomString();
			var password = TestUtils.GenerateRandomString();
			var email = TestUtils.GenerateRandomEmail();

			var isRegistrationComplete = false;
			XsollaAuth.Register(
				username,
				password,
				email,
				_ => isRegistrationComplete = true,
				error => {
					isRegistrationComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isRegistrationComplete);

			var isComplete = false;
			XsollaAuth.ResendConfirmationLink(
				username,
				() => isComplete = true,
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator BlocksRegistrationWhenEmailWasPreviouslyRegistered()
		{
			var isComplete = false;

			var email = TestCredentials.DefaultEmail;
			var username = TestUtils.GenerateRandomString();
			var password = TestUtils.GenerateRandomString();

			XsollaAuth.Register(
				username,
				password,
				email,
				_ => {
					isComplete = true;
					Assert.Fail("Registration should fail when email was previously registered");
				},
				error => {
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}