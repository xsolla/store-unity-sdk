using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Demo;

namespace Xsolla.Tests
{
    public class BattlePassCurrentDefinerTests
    {
		[Test]
		[TestCaseSource(nameof(TestCaseSource))]
		public void OnBattlePassDescriptionsConverted_Descriptions_ReturnsExpectedCurrent(string currentDate, DateTime[] expiryDates, string[] battlePassNames)
		{
			var gameObject = new GameObject();
			var definer = gameObject.AddComponent<BattlePassCurrentDefiner>();
			var testDescriptions = new List<BattlePassDescription>(expiryDates.Length);

			for (int i = 0; i < expiryDates.Length; i++)
			{
				var testDescription = new BattlePassDescription(battlePassNames[i], expiryDates[i], false, null);
				testDescriptions.Add(testDescription);
			}

			var resultDescription = default(BattlePassDescription);

			definer.CurrentBattlePassDefined += current => resultDescription = current;
			definer.OnBattlePassDescriptionsConverted(testDescriptions);

			Assert.AreEqual("EXPECTED", resultDescription.Name);
		}

		private static object[] TestCaseSource = new object[]
		{
			new object[] { "16.02.2021", new DateTime[] { new DateTime(2021, 02, 10), new DateTime(2021, 02, 22), new DateTime(2021, 02, 9), new DateTime(2021, 02, 7), new DateTime(2021, 02, 13),
														new DateTime(2021, 02, 16), new DateTime(2021, 02, 15), new DateTime(2021, 02, 4), new DateTime(2021, 02, 21), new DateTime(2021, 02, 23) },
										 new string[] { null, null, null, null, null, null, null, null, "EXPECTED", null } },

			new object[] { "16.02.2021", new DateTime[] { new DateTime(2021, 02, 9), new DateTime(2021, 02, 14), new DateTime(2021, 02, 7), new DateTime(2021, 02, 5), new DateTime(2021, 02, 10) },
										 new string[] { null, "EXPECTED", null, null, null } },

			new object[] { "16.02.2021", new DateTime[] { new DateTime(2021, 02, 27), new DateTime(2021, 02, 21), new DateTime(2021, 02, 18), new DateTime(2021, 02, 24), new DateTime(2021, 02, 16) },
										 new string[] { null, null, "EXPECTED", null, null, } },

			new object[] { "16.02.2021", new DateTime[] { new DateTime(2021, 02, 27) },
										 new string[] { "EXPECTED" } }
		};
	}
}
