using System;

[Serializable]
public class UserSubscriptionModel : ItemModel
{
	public enum SubscriptionStatusType
	{
		None,
		Active,
		Expired
	}
	
	public override bool IsVirtualCurrency() => false;
	public override bool IsSubscription() => true;

	public SubscriptionStatusType Status { get; set; }
	public DateTime? Expired { get; set; }
}