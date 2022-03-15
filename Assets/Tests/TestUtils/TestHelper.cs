using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;

using Debug = UnityEngine.Debug;

namespace Xsolla.Tests
{
    public class TestHelper
    {
		public static void Pass(string testName)
		{
			Debug.Log($"SUCCESS: '{testName}'");
		}

		public static void Pass(string testName, string additionalInfo)
		{
			Debug.Log($"SUCCESS: '{testName}' info: '{additionalInfo}'");
		}

		public static void Pass(string testName, TestSignInHelper.SignInResult signInResult)
		{
			Debug.Log($"SUCCESS: '{testName}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'");
		}

		public static void Fail(string testName)
		{
			var message = $"FAIL: '{testName}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(string testName, string additionalInfo)
		{
			var message = $"FAIL: '{testName}' info: '{additionalInfo}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(string testName, TestSignInHelper.SignInResult signInResult)
		{
			var message = $"FAIL: '{testName}' signInError: '{signInResult?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		public static void Fail(string testName, Error error)
		{
			var message = $"FAIL: '{testName}' error: '{error?.errorMessage ?? "ERROR IS NULL"}'";
			Debug.LogError(message);
			Assert.Fail(message);
		}

		//public class ResultContainer
		//{
		//	public readonly bool success;
		//	public readonly string errorMessage;

		//	public ResultContainer(bool success, string errorMessage)
		//	{
		//		this.success = success;
		//		this.errorMessage = errorMessage;
		//	}
		//}
	}
}
