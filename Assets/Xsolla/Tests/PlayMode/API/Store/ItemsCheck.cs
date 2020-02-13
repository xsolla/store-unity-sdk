using Xsolla.Store;
using NUnit.Framework;
using System.Linq;
using System;

public static class ItemsCheck
{
	public static void ValidateItems(StoreItems items, Nullable<int> expectedCount = null)
	{
		if(expectedCount != null) {
			CheckItemsCount(items, expectedCount.Value);
		}

		foreach (StoreItem item in items.items.ToList()) {
			if (!ItemsCheck.ValidateItem(item, out string message)) {
				Assert.Fail(message);
			}
		}
	}

	private static void CheckItemsCount(StoreItems items, int expectedCount)
	{
		CompareItemsCount(expectedCount, items.items.Count());
	}

	private static void CompareItemsCount(int expected, int real)
	{
		bool condition = real == expected;
		Assert.True(condition,
			"Items count must be = " + expected +
			" but we have = " + real
		);
	}

	public static bool ValidateItem(StoreItem item, out string message)
	{
		message = "no message";

		Assert.False(string.IsNullOrEmpty(item.sku), "Sku is null or empty");

		return true;
	}

	public static void ValidateItems(VirtualCurrencyItems items, Nullable<int> expectedCount = null)
	{
		if (expectedCount != null) {
			CheckItemsCount(items, expectedCount.Value);
		}

		foreach (StoreItem item in items.items.ToList()) {
			if (!ItemsCheck.ValidateItem(item, out string message)) {
				Assert.Fail(message);
			}
		}
	}

	private static void CheckItemsCount(VirtualCurrencyItems items, int expectedCount)
	{
		CompareItemsCount(expectedCount, items.items.Count());
	}
}
