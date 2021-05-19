using System;

[Serializable]
public class VirtualCurrencyModel : ItemModel
{
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
