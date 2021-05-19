public class CatalogVirtualCurrencyModel : CatalogItemModel
{
	public uint Amount { get; set; }
	public string CurrencySku { get; set; }

	public override bool IsVirtualCurrency()
	{
		return true;
	}

	public override bool IsSubscription()
	{
		return false;
	}

	public override bool IsBundle()
	{
		return false;
	}
}
