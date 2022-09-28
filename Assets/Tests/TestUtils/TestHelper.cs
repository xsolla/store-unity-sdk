using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Cart;
using Xsolla.Inventory;
using Xsolla.GameKeys;
using Xsolla.Orders;
using Xsolla.Subscriptions;
using Xsolla.UserAccount;

using Debug = UnityEngine.Debug;

namespace Xsolla.Tests
{
	public class TestHelper
	{
		public static void Pass([CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}'");
		}

		public static void Pass(string additionalInfo, [CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}' info: '{additionalInfo}'");
		}

		public static void Pass(TestSignInHelper.SignInResult signInResult, [CallerMemberName] string testName = null)
		{
			Debug.Log($"SUCCESS: '{testName}' signInSuccess: '{signInResult?.success}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'");
		}

		public static void Fail([CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(string additionalInfo, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' info: '{additionalInfo}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(TestSignInHelper.SignInResult signInResult, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' signInSuccess: '{signInResult?.success}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(Error error, [CallerMemberName] string testName = null)
		{
			var message = $"FAIL: '{testName}' error: '{error?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void CheckTokenChanged([CallerMemberName] string testName = null)
		{
			string oldToken = TestSignInHelper.Instance.OldToken;
			CheckTokenChanged(oldToken, testName);
		}

		public static void CheckTokenChanged(string oldToken, [CallerMemberName] string testName = null)
		{
			string newToken = Token.Instance;

			if (newToken != oldToken)
				Pass("TOKEN CHANGED", testName);
			else
				Fail("TOKEN DID NOT CHANGE", testName);
		}

		public static void Clear()
		{
			//Clear Token and Refresh Token
			if (Token.Instance != null)
				Token.Instance = null;
			if (TokenRefresh.IsExist)
				GameObject.DestroyImmediate(TokenRefresh.Instance.gameObject);

			PlayerPrefs.DeleteKey(Constants.LAST_SUCCESS_AUTH_TOKEN);
			PlayerPrefs.DeleteKey(Constants.LAST_SUCCESS_OAUTH_REFRESH_TOKEN);

			//Delete temporary objects if any
			if (XsollaAuth.IsExist)
				GameObject.DestroyImmediate(XsollaAuth.Instance.gameObject);
			if (XsollaCart.IsExist)
				GameObject.DestroyImmediate(XsollaCart.Instance.gameObject);
			if (XsollaCatalog.IsExist)
				GameObject.DestroyImmediate(XsollaCatalog.Instance.gameObject);
			if (XsollaGameKeys.IsExist)
				GameObject.DestroyImmediate(XsollaGameKeys.Instance.gameObject);
			if (XsollaInventory.IsExist)
				GameObject.DestroyImmediate(XsollaInventory.Instance.gameObject);
			if (XsollaOrders.IsExist)
				GameObject.DestroyImmediate(XsollaOrders.Instance.gameObject);
			if (XsollaSubscriptions.IsExist)
				GameObject.DestroyImmediate(XsollaSubscriptions.Instance.gameObject);
			if (XsollaUserAccount.IsExist)
				GameObject.DestroyImmediate(XsollaUserAccount.Instance.gameObject);

			//Delete temporary util objects if any
			if (TestSignInHelper.IsExist)
				GameObject.DestroyImmediate(TestSignInHelper.Instance.gameObject);
			if (WebRequestHelper.IsExist)
				GameObject.DestroyImmediate(WebRequestHelper.Instance.gameObject);
		}
	}
}