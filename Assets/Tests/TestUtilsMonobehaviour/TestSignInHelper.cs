using System;
using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class TestSignInHelper : MonoSingleton<TestSignInHelper>
    {
		public bool IsSignedIn => Token.Instance != null;

		public void Setup()
		{
			//Prevent OAuth Init
			XsollaSettings.AuthorizationType = AuthorizationType.JWT;

			XsollaAuth.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
			XsollaAuth.Instance.DeleteToken(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);
			XsollaAuth.Instance.DeleteToken(Constants.OAUTH_REFRESH_TOKEN_EXPIRATION_TIME);

			Token.Instance = null;

			XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;
		}

		public void TearDown()
		{
			if (XsollaAuth.IsExist)
				UnityEngine.Object.DestroyImmediate(XsollaAuth.Instance.gameObject);

			Token.Instance = null;
		}

		public IEnumerator SignIn(string login = "xsolla", string password = "xsolla", Action<SignInResult> callback = null)
		{
			bool? success = default;
			string errorMessage = default;

			Action<string> onSuccess = _ => success = true;
			Action<Error> onError = error => { errorMessage = error?.errorMessage ?? "ERROR IS NULL"; success = false; };

			XsollaAuth.Instance.SignIn(login, password, false, null, null, onSuccess, onError);

			yield return new WaitUntil(() => success.HasValue);

			callback?.Invoke(new SignInResult(success.Value, errorMessage));
		}

		public class SignInResult
		{
			public readonly bool success;
			public readonly string errorMessage;

			public SignInResult(bool success, string errorMessage)
			{
				this.success = success;
				this.errorMessage = errorMessage;
			}
		}
	}
}
