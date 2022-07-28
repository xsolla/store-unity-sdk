using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaAuthUserTests
	{
		[UnityTest]
		public IEnumerator SignIn_OAuth_Success()
		{
			yield return SignIn();
		}

		[UnityTest]
		public IEnumerator SignIn_OAuth_IncorrectData_Failure()
		{
			yield return SignIn(login:"k4TCNgHs", password:"k4TCNgHs", isSuccessExpected: false);
		}

		private IEnumerator SignIn([CallerMemberName] string testName = null, string login = "xsolla", string password = "xsolla", bool isSuccessExpected = true)
		{
			TestSignInHelper.SignInResult signInResult = null;

			yield return TestSignInHelper.Instance.SignIn(login, password, result => signInResult = result);
			yield return new WaitUntil(() => signInResult != null);

			if (signInResult.success == isSuccessExpected)
				TestHelper.Pass(signInResult,testName);
			else
				TestHelper.Fail(signInResult,testName);
		}

		[UnityTest]
		public IEnumerator GetUserInfo_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = null;
			Error getInfoError = null;

			XsollaAuth.Instance.GetUserInfo(
				token: Token.Instance,
				onSuccess: userInfo => success = userInfo != null,
				onError: error =>
				{
					success = false;
					getInfoError = error;
				});
			
			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass();
			else
				TestHelper.Fail(getInfoError);
		}

		//There is a limit for this request, so test only one arguments combination - high risk of request error because of backend rules
		[UnityTest]
		public IEnumerator StartAuthByPhoneNumber_Success()
		{
			bool? success = null;
			string operationID = null;
			Error startAuthError = null;

			XsollaAuth.Instance.StartAuthByPhoneNumber(
				phoneNumber: "+79008007060",
				linkUrl: null,
				sendLink: false,
				onSuccess: opID =>
				{
					operationID = opID;
					success = true;
				},
				onError: error =>
				{
					startAuthError = error;
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(additionalInfo: operationID);
			else
				TestHelper.Fail(error: startAuthError);
		}

		[UnityTest]
		public IEnumerator OAuthLogout_Sso_Success()
		{
			yield return OAuthLogout(logoutType: OAuthLogoutType.Sso);
		}

		[UnityTest]
		public IEnumerator OAuthLogout_All_Success()
		{
			yield return OAuthLogout(logoutType: OAuthLogoutType.All);
		}

		private IEnumerator OAuthLogout([CallerMemberName]string testName = null, OAuthLogoutType logoutType = OAuthLogoutType.All)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = null;
			Error getInfoError = null;

			XsollaAuth.Instance.OAuthLogout(
				token: Token.Instance,
				sessions: logoutType,
				onSuccess: () => success = true,
				onError: error =>
				{
					success = false;
					getInfoError = error;
				});
			
			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
			{
				TestHelper.Pass(testName);
				Token.Instance = null;
			}
			else
				TestHelper.Fail(getInfoError, testName);
		}

		[UnityTest]
		public IEnumerator ResetPassword_DefaultValues_Success()
		{
			yield return ResetPassword();
		}

		[UnityTest]
		public IEnumerator ResetPassword_de_DE_Locale_Success()
		{
			yield return ResetPassword(locale:"de_DE");
		}

		private IEnumerator ResetPassword([CallerMemberName]string testName = null, string redirectUri = null, string locale = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			string userEmail = null;
			Error getInfoError = null;
			bool busy = true;

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
				TestHelper.Fail(getInfoError, testName);
				yield break;
			}
			if (userEmail == null)
			{
				TestHelper.Fail("UserEmail is NULL", testName);
				yield break;
			}

			bool? success = null;
			Error resetPasswordError = null;

			XsollaAuth.Instance.ResetPassword(
				email: userEmail,
				redirectUri: redirectUri,
				locale: locale,
				onSuccess: () => success = true,
				onError: error => 
				{
					resetPasswordError = error;
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass();
			else
				TestHelper.Fail(error: resetPasswordError);
		}

		[UnityTest]
		public IEnumerator ResendConfirmationLink_DefaultValues_Success()
		{
			yield return ResendConfirmationLink();
		}

		[UnityTest]
		public IEnumerator ResendConfirmationLink_de_DE_Locale_Success()
		{
			yield return ResendConfirmationLink(locale:"de_DE");
		}

		private IEnumerator ResendConfirmationLink([CallerMemberName]string testName = null, string redirectUri = null, string state = null, string payload = null, string locale = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			string userEmail = null;
			Error getInfoError = null;
			bool busy = true;

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
				TestHelper.Fail(getInfoError, testName);
				yield break;
			}
			if (userEmail == null)
			{
				TestHelper.Fail("UserEmail is NULL", testName);
				yield break;
			}

			bool? success = null;
			Error resendLinkError = null;

			XsollaAuth.Instance.ResendConfirmationLink(
				username: userEmail,
				redirectUri: redirectUri,
				state: state,
				payload: payload,
				locale: locale,
				onSuccess: () => success = true,
				onError: error => 
				{
					resendLinkError = error;
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass();
			else
				TestHelper.Fail(error: resendLinkError);
		}
	}
}