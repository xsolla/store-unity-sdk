using System;
using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class TestSignInHelper : MonoSingleton<TestSignInHelper>
	{
		private const string OLD_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOltdLCJlbWFpbCI6Inhzb2xsYXN0b3Jlc2RrLmFAZ21haWwuY29tIiwiZXhwIjoxNjQ4NDYwMzc2LCJncm91cHMiOlt7ImlkIjo2NDgxLCJuYW1lIjoiZGVmYXVsdCIsImlzX2RlZmF1bHQiOnRydWV9XSwiaWF0IjoxNjQ4MzczOTc2LCJpc19tYXN0ZXIiOnRydWUsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImp0aSI6ImFhNzRhMTVhLWIxNWQtNGM4MS1hYzUxLTdmNGE1OTVkYWU5YyIsInByb21vX2VtYWlsX2FncmVlbWVudCI6dHJ1ZSwicHVibGlzaGVyX2lkIjoxMzY1OTMsInNjcCI6WyJvZmZsaW5lIl0sInN1YiI6IjE0ZGNiMzQzLWFkMjUtNDZhOS05YjE2LWJjZWVjNWNmN2NjZiIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ1c2VybmFtZSI6Inhzb2xsYSIsInhzb2xsYV9sb2dpbl9hY2Nlc3Nfa2V5Ijoialpta3dObUlsZ25IZVVCSE8xRURRUFhleHJhdjJYdEk4Y1FJeF90ajliZyIsInhzb2xsYV9sb2dpbl9wcm9qZWN0X2lkIjoiMDI2MjAxZTMtN2U0MC0xMWVhLWE4NWItNDIwMTBhYTgwMDA0In0.Y4qJ0_UStkND4Xsjhy1P0EUv1DLswxybxLKW8IV6N3Y";

		public string OldToken => OLD_TOKEN;

		public IEnumerator CheckSession()
		{
			if (Token.Instance == null ||
				string.IsNullOrEmpty(TokenRefresh.Instance.RefreshToken) ||
				XsollaSettings.AuthorizationType != AuthorizationType.OAuth2_0 ||
				!XsollaAuth.IsExist)
			{
				XsollaSettings.AuthorizationType = AuthorizationType.OAuth2_0;
				yield return TestSignInHelper.Instance.SignIn();
			}
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

		public IEnumerator SignOut()
		{
			if (Token.Instance == null ||
				XsollaSettings.AuthorizationType != AuthorizationType.OAuth2_0)
				yield break;

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () => success = true;
			Action<Error> onSignOutError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL"; success = false;
				UnityEngine.Debug.LogError(errorMessage);
			};

			XsollaAuth.Instance.OAuthLogout(
				token: Token.Instance,
				sessions: OAuthLogoutType.All,
				onSuccess: onSuccess,
				onError: onSignOutError);

			yield return new WaitUntil(() => success.HasValue);
			Token.Instance = null;
			TokenRefresh.Instance.RefreshToken = string.Empty;
		}

		public IEnumerator SetOldToken()
		{
			yield return GenerateSession();
			Token.Instance = Token.Create(OLD_TOKEN);
		}

		public IEnumerator GenerateSession()
		{
			XsollaAuth.Instance.Init();
			yield return SignOut();
			yield return CheckSession();
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
