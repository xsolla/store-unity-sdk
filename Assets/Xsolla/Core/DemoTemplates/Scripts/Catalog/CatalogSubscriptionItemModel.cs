using System;

public class CatalogSubscriptionItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency() => false;
	public override bool IsSubscription() => true;
	
	public TimeSpan ExpirationPeriod { get; set; }
	public string ExpirationPeriodText { get; set; }
}