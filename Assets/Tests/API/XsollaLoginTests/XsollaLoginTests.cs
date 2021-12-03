using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Login;

namespace Tests
{
    public class XsollaLoginTests
    {
		[UnityTest]
		public IEnumerator SignIn_DemoUser_Success()
		{
			bool? success = default;
			string errorMessage = default;

			Action<string> onSuccess = _ => success = true;
			Action<Error> onError = error => { errorMessage = error.errorMessage; success = false; };

			XsollaLogin.Instance.SignIn("xsolla", "xsolla", false, null, onSuccess, onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				Assert.Pass();
			else
				Assert.Fail(errorMessage);
		}

		[UnityTest]
		public IEnumerator SignIn_RandomLoginPass_Failure()
		{
			bool? success = default;
			string errorMessage = default;

			Action<string> onSuccess = _ => success = true;
			Action<Error> onError = error => { errorMessage = error.errorMessage; success = false; };

			XsollaLogin.Instance.SignIn("asdfsdf", "pcwefd", false, null, onSuccess, onError);

			yield return new WaitUntil(() => success.HasValue);

			if (!success.Value)
				Assert.Pass(errorMessage);
			else
				Assert.Fail(errorMessage);
		}

		[UnityTest]
		public IEnumerator GetUserInfo_DemoUserToken_Success()
		{
			bool? success = default;
			string errorMessage = default;
			string demoUserToken = default;

			Action<string> onSuccessLogin = token => { demoUserToken = token; success = true; };
			Action<Error> onErrorLogin = error => { errorMessage = error.errorMessage; success = false; };
			XsollaLogin.Instance.SignIn("xsolla", "xsolla", false, null, onSuccessLogin, onErrorLogin);

			yield return new WaitUntil(() => success.HasValue);

			if (!success.Value)
				Assert.Fail(errorMessage);

			success = default;
			Action<UserInfo> onSuccess = _ => success = true;
			Action<Error> onError = error => { errorMessage = error.errorMessage; success = false; };
			XsollaLogin.Instance.GetUserInfo(demoUserToken, onSuccess, onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				Assert.Pass();
			else
				Assert.Fail(errorMessage);
		}
	}
}
