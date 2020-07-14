using System;

[Serializable]
public class VirtualCurrencyBalanceModel : ItemModel
{
	public override bool IsVirtualCurrency() => true;
	public uint Amount { get; set; }
}