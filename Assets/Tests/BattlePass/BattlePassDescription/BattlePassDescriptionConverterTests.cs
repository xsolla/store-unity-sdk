using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Demo;

namespace Xsolla.Tests
{
    public class BattlePassDescriptionConverterTests
    {
		private BattlePassDescriptionConverter _converter;

		[SetUp]
		public void Setup()
		{
			var gameObject = new GameObject();
			_converter = gameObject.AddComponent<BattlePassDescriptionConverter>();
		}

		[Test]
		[TestCase("02.16.2021", "02.17.2021", true)]
		[TestCase("02.17.2021", "02.17.2021", true)]
		[TestCase("02.18.2021", "02.17.2021", false)]
		public void CheckIfSoonerOrEqual_DateTimes_ReturnsExpected(string expiryDateAsString, string currentDateAsString, bool expected)
		{
			var type = typeof(BattlePassDescriptionConverter);
			var method = type.GetMethod("CheckIfSoonerOrEqual", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

			var expiryDate = DateTime.Parse(expiryDateAsString, CultureInfo.InvariantCulture);
			var currentDate = DateTime.Parse(currentDateAsString, CultureInfo.InvariantCulture);

			var result = (bool)method.Invoke(_converter, new object[] { expiryDate, currentDate });

			Assert.AreEqual(expected, result);
		}

		[Test]
		[TestCase("17-02-2021", true)]
		[TestCase("06-06-2021", true)]
		[TestCase("17.02.2021", false)]
		public void ConvertDateTime_DateTimes_ReturnsExpected(string dateTimeToConvert, bool expected)
		{
			//Test fails on Debug.LogError in original code - this disables such behaviour
			LogAssert.ignoreFailingMessages = true;

			var type = typeof(BattlePassDescriptionConverter);
			var method = type.GetMethod("ConvertDateTime", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

			var result = (bool)method.Invoke(_converter, new object[] { dateTimeToConvert, DateTime.Now });

			Assert.AreEqual(expected, result);
		}

		[Test]
		[TestCase("BP2021JAN")]
		[TestCase("BP2021FEB")]
		[TestCase("BPTEST")]
		public void OnBattlePassJsonExtracted_Descriptions_ExpectedName(string BPname)
		{
			var testDescription = new BattlePassDescriptionRaw() { Name = BPname, ExpiryDate = "30-12-2020" };
			var resultDescription = new List<BattlePassDescription>();

			_converter.BattlePassDescriptionsConverted += result => resultDescription.AddRange(result);
			_converter.OnBattlePassJsonExtracted( new List<BattlePassDescriptionRaw>() { testDescription } );

			Assert.AreEqual(BPname, resultDescription[0].Name);
		}

		[Test]
		[TestCase("30-12-2020")]
		[TestCase("01-01-2021")]
		[TestCase("20-10-2022")]
		public void OnBattlePassJsonExtracted_Descriptions_ExpectedExpiryDate(string expiryDate)
		{
			var testDescription = new BattlePassDescriptionRaw() { ExpiryDate = expiryDate };
			var resultDescription = new List<BattlePassDescription>();
			var expectedExpiryDate = DateTime.ParseExact(expiryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

			_converter.BattlePassDescriptionsConverted += result => resultDescription.AddRange(result);
			_converter.OnBattlePassJsonExtracted(new List<BattlePassDescriptionRaw>() { testDescription });

			Assert.AreEqual(expectedExpiryDate.Day, resultDescription[0].ExpiryDate.Day);
			Assert.AreEqual(expectedExpiryDate.Month, resultDescription[0].ExpiryDate.Month);
			Assert.AreEqual(expectedExpiryDate.Year, resultDescription[0].ExpiryDate.Year);
		}
	}
}
