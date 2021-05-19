using System;

public class CatalogSubscriptionItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency()
	{
		return false;
	}

	public override bool IsSubscription()
	{
		return true;
	}

	public override bool IsBundle()
	{
		return false;
	}
	
	public TimeSpan ExpirationPeriod { get; set; }
	public string ExpirationPeriodText { get; set; }
}
