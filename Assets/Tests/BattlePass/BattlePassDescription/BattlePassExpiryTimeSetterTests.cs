using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Demo;

namespace Xsolla.Tests
{
    public class BattlePassExpiryTimeSetterTests
    {
		private BattlePassExpiryTimeSetter _expiryTimeSetter;

		[SetUp]
		public void Setup()
		{
			var gameObject = new GameObject();
			_expiryTimeSetter = gameObject.AddComponent<BattlePassExpiryTimeSetter>();
		}

		private MethodInfo GetMethod(string methodName)
		{
			var type = typeof(BattlePassExpiryTimeSetter);
			var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

			return method;
		}

		[Test]
		[TestCase("16.02.2021 10:00:00", "16.02.2021 15:50:00", 6)]
		[TestCase("16.02.2021 10:00:00", "16.02.2021 16:00:00", 6)]
		[TestCase("16.02.2021 16:00:00", "16.02.2021 10:00:00", 6)]
		[TestCase("16.02.2021 16:00:00", "16.02.2021 16:00:00", 0)]
		[TestCase("16.02.2021 16:00:00", "16.02.2021 16:20:00", 1)]
		[TestCase("16.02.2021 16:00:00", "17.02.2021 16:00:00", 24)]
		[TestCase("16.02.2021 16:00:00", "17.02.2021 16:10:00", 25)]
		public void GetDifferenceInHours_DateTimes_ReturnsExpectedHours(string dateTimeAAsString, string dateTimeBAsString, int expectedHours)
		{
			var dateTimeA = DateTime.ParseExact(dateTimeAAsString, "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));
			var dateTimeB = DateTime.ParseExact(dateTimeBAsString, "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));

			var method = GetMethod("GetDifferenceInHours");

			var result = (int)method.Invoke(_expiryTimeSetter, new object[] { dateTimeA, dateTimeB });

			Assert.AreEqual(expectedHours, result);
		}

		[Test]
		[TestCase(1, "Ends in 1h")]
		[TestCase(10, "Ends in 10h")]
		[TestCase(24, "Ends in 1d 0h")]
		[TestCase(25, "Ends in 1d 1h")]
		[TestCase(49, "Ends in 2d 1h")]
		public void FormatExpirationText_ExpirationHours_ExpectedText(int expirationHours, string expectedText)
		{
			var method = GetMethod("FormatExpirationText");
			var result = (string)method.Invoke(_expiryTimeSetter, new object[] { expirationHours });

			Assert.AreEqual(expectedText, result);
		}
	}
}
