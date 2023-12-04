using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Tests.Auth
{
	public class AccountLinkingTests : AuthTestsBase
	{
		// [UnityTest]
		public IEnumerator SignInConsoleAccount_NintendoShop_Success()
		{
			yield return SignInConsoleAccount("nintendo_shop");
		}

		// [UnityTest]
		public IEnumerator SignInConsoleAccount_XBoxLive_Success()
		{
			yield return SignInConsoleAccount("xbox_live");
		}

		private static IEnumerator SignInConsoleAccount(string platform)
		{
			yield return SignOut();

			var currentTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			var digits = new List<char>();
			foreach (var symbol in currentTime)
			{
				if (char.IsDigit(symbol))
					digits.Add(symbol);
			}

			var account = $"TestAcc{new string(digits.ToArray())}";

			var isComplete = false;
			XsollaAuth.SignInConsoleAccount(
				account,
				platform,
				() =>
				{
					isComplete = true;
					Assert.NotNull(XsollaToken.AccessToken);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}