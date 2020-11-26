using System;

[Serializable]
public class VirtualCurrencyBalanceModel : ItemModel
{
	public override bool IsVirtualCurrency() => true;
	public override bool IsSubscription() => false;
	public override bool IsBundle() => false;

	public uint Amount { get; set; }
}