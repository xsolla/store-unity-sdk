using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using System.Collections.Generic;

namespace Xsolla.Tests
{
	public class XsollaAuthAccountLinkingTests
	{
		[UnityTest]
		public IEnumerator SignInConsoleAccount_nintendo_shop_Success()
		{
			yield return SignInConsoleAccount(platform:"nintendo_shop");
		}

		[UnityTest]
		public IEnumerator SignInConsoleAccount_xbox_live_Success()
		{
			yield return SignInConsoleAccount(platform:"xbox_live");
		}

		private IEnumerator SignInConsoleAccount([CallerMemberName]string testName = null, string platform = "")
		{
			yield return TestSignInHelper.Instance.SignOut();

			var currentTime = DateTime.Now.ToString();
			var digits = new List<char>();
			foreach (var symbol in currentTime)
			{
				if (char.IsDigit(symbol))
					digits.Add(symbol);
			}
			var testAccount = $"TestAcc{new string(digits.ToArray())}";

			UnityEngine.Debug.Log($"Console test account: {testAccount}");

			bool? success = default;
			string consoleToken = default;
			string errorMessage = default;

			XsollaAuth.Instance.SignInConsoleAccount(
				userId: testAccount,
				platform: platform,
				successCase: token =>
				{
					consoleToken = token;
					success = true;
				},
				failedCase: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value && !string.IsNullOrEmpty(consoleToken))
				TestHelper.Pass(consoleToken, testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}
	}
}