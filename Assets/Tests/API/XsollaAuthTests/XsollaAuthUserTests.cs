using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaAuthUserTests
    {
		[SetUp]
		public void Setup() => TestSignInHelper.Instance.Setup();

		[TearDown]
		public void TearDown() => TestSignInHelper.Instance.TearDown();

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

		//There is a limit for this request, so test only one arguments combination - high risk of request error because of backend rules otherwise
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
	}
}
