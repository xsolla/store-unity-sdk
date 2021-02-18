using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Demo;
using System.Linq;

namespace Xsolla.Tests
{
    public class BattlePassJsonExtractorTests
    {
		private BattlePassJsonExtractor _jsonExtractor;
		private List<BattlePassDescriptionRaw> _rawDescriptions;

		[SetUp]
		public void SetupTestSubjects()
		{
			var gameObject = new GameObject();
			_jsonExtractor = gameObject.AddComponent<BattlePassJsonExtractor>();

			_rawDescriptions = new List<BattlePassDescriptionRaw>();

			_jsonExtractor.BattlePassJsonExtracted += extractedDescriptions => _rawDescriptions.AddRange(extractedDescriptions);
		}

		private void SetupTestCatalogItems(string[] testDescriptions, out List<CatalogItemModel> testCatalogItemModels)
		{
			testCatalogItemModels = new List<CatalogItemModel>(testDescriptions.Length);

			foreach (var testDescription in testDescriptions)
			{
				var catalogModel = new CatalogVirtualItemModel();
				catalogModel.LongDescription = testDescription;
				testCatalogItemModels.Add(catalogModel);
			}
		}

		[Test]
		[TestCase(new string[0], 0)]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionA }, 1)]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionB }, 1)]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionA, TESTBATTLEPASSDESCRIPTIONS.DescriptionB }, 2)]
		public void OnBattlePassItemsExtracted_Items_ExpectedCount(string[] battlePassDescriptions, int expectedCount)
        {
			SetupTestCatalogItems(battlePassDescriptions, out var testCatalogItemModels);

			_jsonExtractor.OnBattlePassItemsExtracted(testCatalogItemModels);

			Assert.AreEqual(expectedCount, _rawDescriptions.Count);
        }

		[Test]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionA }, 3)]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionB }, 3)]
		public void OnBattlePassItemsExtracted_Items_ExpectedLevelCount(string[] battlePassDescriptions, int expectedLevelCount)
		{
			SetupTestCatalogItems(battlePassDescriptions, out var testCatalogItemModels);

			_jsonExtractor.OnBattlePassItemsExtracted(testCatalogItemModels);

			foreach (var description in _rawDescriptions)
			{
				Assert.AreEqual(expectedLevelCount, description.Levels.Length);
			}
		}

		[Test]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionA }, "BP2021JAN")]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionB }, "BP2021JAN")]
		public void OnBattlePassItemsExtracted_Items_ExpectedBattlePassName(string[] battlePassDescriptions, string expectedName)
		{
			SetupTestCatalogItems(battlePassDescriptions, out var testCatalogItemModels);

			_jsonExtractor.OnBattlePassItemsExtracted(testCatalogItemModels);

			foreach (var description in _rawDescriptions)
			{
				Assert.AreEqual(expectedName, description.Name);
			}
		}

		[Test]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionA }, 1000)]
		[TestCase(new string[] { TESTBATTLEPASSDESCRIPTIONS.DescriptionB }, 100)]
		public void OnBattlePassItemsExtracted_Items_ExpectedLevelExperience(string[] battlePassDescriptions, int expectedLevelExperience)
		{
			SetupTestCatalogItems(battlePassDescriptions, out var testCatalogItemModels);

			_jsonExtractor.OnBattlePassItemsExtracted(testCatalogItemModels);

			foreach (var description in _rawDescriptions)
			{
				Assert.AreEqual(expectedLevelExperience, description.Levels[0].Experience);
			}
		}
	}
}
