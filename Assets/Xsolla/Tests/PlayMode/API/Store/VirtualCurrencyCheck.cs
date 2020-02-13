using Xsolla.Store;
using NUnit.Framework;
using System.Linq;
using System;
using System.Collections.Generic;

public static class VirtualCurrencyCheck
{
	public static void ValidateBalance(VirtualCurrencyBalance balance)
	{
		Assert.That(!string.IsNullOrEmpty(balance.sku), () => "Virtual currency's sku can not be null or empty!");
		Assert.That(!string.IsNullOrEmpty(balance.type), () => "Virtual currency's type can not be null or empty!");
		Assert.AreEqual(balance.type, "virtual_currency");
	}

	public static void ValidateBalance(VirtualCurrenciesBalance balance)
	{
		balance.items.ToList().ForEach(b => ValidateBalance(b));
	}

	public static void ValidatePackages(VirtualCurrencyPackages packages)
	{
		packages.items.ForEach(p => ValidatePackage(p));
	}

	public static void ValidatePackage(VirtualCurrencyPackage package)
	{
		ItemsCheck.ValidateItem(package, out string message);

		ValidatePackageContents(package.content);

		Assert.That(!string.IsNullOrEmpty(package.bundle_type), () => "Virtual currency's type can not be null or empty!");
		Assert.AreEqual(package.bundle_type, "virtual_currency_package");
	}

	private static void ValidatePackageContents(List<VirtualCurrencyPackage.Content> content)
	{
		Assert.That(content.Count > 0, "Virtual currency package can not be empty");
		content.ForEach(c => ValidatePackageContent(c));
	}

	private static void ValidatePackageContent(VirtualCurrencyPackage.Content content)
	{
		Assert.True(!string.IsNullOrEmpty(content.sku));
		Assert.True(!string.IsNullOrEmpty(content.type));
		Assert.True(content.type == "virtual_currency");
	}
}
