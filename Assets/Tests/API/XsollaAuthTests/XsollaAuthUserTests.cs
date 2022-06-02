using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaAuthUserTests
    {
		[SetUp]
		[TearDown]
		public void Setup()
		{
			if (XsollaSettings.AuthorizationType != AuthorizationType.OAuth2_0)
				XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;
		}

		[UnityTest]
		public IEnumerator SignIn_JWT_Success()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.JWT;
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(SignIn_JWT_Success);

				if (result.success)
				{
					if (Token.Instance != null)
						TestHelper.Pass(testName);
					else
						TestHelper.Fail(testName, "TOKEN IS NULL");
				}
				else
					TestHelper.Fail(testName, result);
			});
		}

		[UnityTest]
		public IEnumerator SignIn_OAuth_Success()
		{
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(SignIn_OAuth_Success);

				if (result.success)
				{
					if (Token.Instance != null)
						TestHelper.Pass(testName);
					else
						TestHelper.Fail(testName, "TOKEN IS NULL");
				}
				else
					TestHelper.Fail(testName, result);
			});
		}

		[UnityTest]
		public IEnumerator SignIn_JWT_IncorrectData_Failure()
		{
			XsollaSettings.AuthorizationType = AuthorizationType.JWT;
			yield return TestSignInHelper.Instance.SignIn("k4TCNgHs", "k4TCNgHs", result =>
			{
				var testName = nameof(SignIn_JWT_IncorrectData_Failure);

				if (!result.success)
					TestHelper.Pass(testName, result);
				else
					TestHelper.Fail(testName);
			});
		}

		[UnityTest]
		public IEnumerator SignIn_OAuth_IncorrectData_Failure()
		{
			yield return TestSignInHelper.Instance.SignIn("k4TCNgHs", "k4TCNgHs", result =>
			{
				var testName = nameof(SignIn_OAuth_IncorrectData_Failure);

				if (!result.success)
					TestHelper.Pass(testName, result);
				else
					TestHelper.Fail(testName);
			});
		}

		[UnityTest]
		public IEnumerator GetUserInfo_Success()
		{
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(GetUserInfo_Success);

				if (!result.success || Token.Instance == null)
				{
					TestHelper.Fail(testName, result);
					return;
				}

				XsollaAuth.Instance.GetUserInfo(
					token: Token.Instance,
					onSuccess: userInfo =>
					{
						if (userInfo != null)
							TestHelper.Pass(testName, userInfo?.email ?? "EMPTY EMAIL");
						else
							TestHelper.Fail(testName, "userInfo is NULL");
					},
					onError: error =>
					{
						TestHelper.Fail(testName, error);
					});
			});
		}

		//There is a limit for this request, so test only one arguments combination - high risk of request error because of backend rules
		[UnityTest]
		public IEnumerator StartAuthByPhoneNumber_Success()
		{
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(StartAuthByPhoneNumber_Success);

				if (!result.success || Token.Instance == null)
				{
					TestHelper.Fail(testName, result);
					return;
				}

				XsollaAuth.Instance.StartAuthByPhoneNumber(
					phoneNumber: "+79008007060",
					linkUrl: null,
					sendLink: false,
					onSuccess: operationID => TestHelper.Pass(testName, operationID),
					onError: error => TestHelper.Fail(testName, error));
			});
		}

		[UnityTest]
		public IEnumerator OAuthLogout_Sso_Success()
		{
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(OAuthLogout_Sso_Success);

				if (!result.success || Token.Instance == null)
				{
					TestHelper.Fail(testName, result);
					return;
				}

				XsollaAuth.Instance.OAuthLogout(
					token: Token.Instance,
					sessions: OAuthLogoutType.Sso,
					onSuccess: () => TestHelper.Pass(testName),
					onError: error => TestHelper.Fail(testName, error));
			});
		}

		[UnityTest]
		public IEnumerator OAuthLogout_All_Success()
		{
			yield return TestSignInHelper.Instance.SignIn(callback: result =>
			{
				var testName = nameof(OAuthLogout_All_Success);

				if (!result.success || Token.Instance == null)
				{
					TestHelper.Fail(testName, result);
					return;
				}

				XsollaAuth.Instance.OAuthLogout(
					token: Token.Instance,
					sessions: OAuthLogoutType.All,
					onSuccess: () => TestHelper.Pass(testName),
					onError: error => TestHelper.Fail(testName, error));
			});
		}

		[UnityTest]
		public IEnumerator ResetPassword_DefaultValues_Success()
		{
			yield return ResetPassword_TestCore(nameof(ResetPassword_DefaultValues_Success));
		}

		[UnityTest]
		public IEnumerator ResetPassword_de_DE_Locale_Success()
		{
			yield return ResetPassword_TestCore(nameof(ResetPassword_de_DE_Locale_Success), locale:"de_DE");
		}

		private IEnumerator ResetPassword_TestCore(string testName, string redirectUri = null, string locale = null)
		{
			
			var signInResult = default(TestSignInHelper.SignInResult);
			yield return TestSignInHelper.Instance.SignIn(callback: result => signInResult = result);
			yield return new WaitWhile(() => signInResult == null);

			if (!signInResult.success || Token.Instance == null)
				TestHelper.Fail(testName, signInResult);

			var userEmail = default(string);
			var getInfoError = default(Error);
			var busy = true;
			XsollaAuth.Instance.GetUserInfo(
				token: Token.Instance,
				onSuccess: userInfo =>
				{
					userEmail = userInfo?.email;
					busy = false;
				},
				onError: error =>
				{
					getInfoError = error;
					busy = false;
				});

			yield return new WaitWhile(() => busy);
			if (getInfoError != null)
			{
				TestHelper.Fail(testName, getInfoError);
				yield break;
			}
			if (userEmail == null)
			{
				TestHelper.Fail(testName, "UserEmail is NULL");
				yield break;
			}

			XsollaAuth.Instance.ResetPassword(
				email: userEmail,
				redirectUri: redirectUri,
				locale: locale,
				onSuccess: () => TestHelper.Pass(testName),
				onError: error => TestHelper.Fail(testName, error));
		}

		[UnityTest]
		public IEnumerator ResendConfirmationLink_DefaultValues_Success()
		{
			yield return ResendConfirmationLink_TestCore(nameof(ResendConfirmationLink_DefaultValues_Success));
		}

		[UnityTest]
		public IEnumerator ResendConfirmationLink_de_DE_Locale_Success()
		{
			yield return ResendConfirmationLink_TestCore(nameof(ResendConfirmationLink_de_DE_Locale_Success), locale:"de_DE");
		}

		private IEnumerator ResendConfirmationLink_TestCore(string testName, string redirectUri = null, string state = null, string payload = null, string locale = null)
		{
			
			var signInResult = default(TestSignInHelper.SignInResult);
			yield return TestSignInHelper.Instance.SignIn(callback: result => signInResult = result);
			yield return new WaitWhile(() => signInResult == null);

			if (!signInResult.success || Token.Instance == null)
				TestHelper.Fail(testName, signInResult);

			var userEmail = default(string);
			var getInfoError = default(Error);
			var busy = true;
			XsollaAuth.Instance.GetUserInfo(
				token: Token.Instance,
				onSuccess: userInfo =>
				{
					userEmail = userInfo?.email;
					busy = false;
				},
				onError: error =>
				{
					getInfoError = error;
					busy = false;
				});

			yield return new WaitWhile(() => busy);
			if (getInfoError != null)
			{
				TestHelper.Fail(testName, getInfoError);
				yield break;
			}
			if (userEmail == null)
			{
				TestHelper.Fail(testName, "UserEmail is NULL");
				yield break;
			}

			XsollaAuth.Instance.ResendConfirmationLink(
				username: userEmail,
				redirectUri: redirectUri,
				state: state,
				payload: payload,
				locale: locale,
				onSuccess: () => TestHelper.Pass(testName),
				onError: error => TestHelper.Fail(testName, error));
		}
	}
}
