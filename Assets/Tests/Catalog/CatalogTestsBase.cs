using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Tests.Catalog
{
	public class CatalogTestsBase : TestBase
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear()
		{
			ClearEnv();
		}

		private const string PERSONALIZED_ITEM_SKU = "xsollus_personalized_item";
		private const string PROMOTION_ITEM_SKU = "sdk_team_special";

		protected static void CheckPersonalization(IEnumerable<BundleItem> items, bool exptected)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku == "sdk_special_pack_forxsollus");
			if (exptected)
				Assert.NotNull(itemWithPromotion);
			else
				Assert.Null(itemWithPromotion);
		}

		protected static void CheckPersonalization(IEnumerable<VirtualCurrencyPackage> items, bool exptected)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku == "crystal_pack_specialxsollus");
			if (exptected)
				Assert.NotNull(itemWithPromotion);
			else
				Assert.Null(itemWithPromotion);
		}

		protected static void CheckPersonalization(IEnumerable<StoreShortItem> items, bool exptected)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku == PERSONALIZED_ITEM_SKU);
			if (exptected)
				Assert.NotNull(itemWithPromotion);
			else
				Assert.Null(itemWithPromotion);
		}

		protected static void CheckPersonalization(IEnumerable<StoreItem> items, bool expected)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku == PERSONALIZED_ITEM_SKU);
			if (expected)
				Assert.NotNull(itemWithPromotion);
			else
				Assert.Null(itemWithPromotion);
		}

		protected static void CheckPromotion(IEnumerable<StoreItem> items, bool expected)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku == PROMOTION_ITEM_SKU);
			if (!expected)
			{
				Assert.Null(itemWithPromotion);
				return;
			}

			Assert.NotNull(itemWithPromotion);

			var promotionContainer = itemWithPromotion.promotions;
			Assert.NotNull(promotionContainer);
			Assert.IsTrue(promotionContainer.Length > 0);

			var promotionItem = promotionContainer[0];
			Assert.NotNull(promotionItem);
			Assert.IsFalse(string.IsNullOrEmpty(promotionItem.date_start));
			Assert.IsFalse(string.IsNullOrEmpty(promotionItem.date_end));
		}
	}
}