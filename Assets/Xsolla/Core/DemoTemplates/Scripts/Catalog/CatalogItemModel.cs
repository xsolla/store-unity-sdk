using System.Collections.Generic;

public abstract class CatalogItemModel : ItemModel
{
	public KeyValuePair<string, float>? RealPrice { get; set; }
	public KeyValuePair<string, float>? RealPriceWithoutDiscount { get; set; }
	public KeyValuePair<string, uint>? VirtualPrice { get; set; }
	public KeyValuePair<string, uint>? VirtualPriceWithoutDiscount { get; set; }

	public KeyValuePair<string, float>? Price
	{
		get
		{
			return VirtualPrice.HasValue
				? new KeyValuePair<string, float>(VirtualPrice.Value.Key, VirtualPrice.Value.Value)
				: RealPrice;
		}
	}

	public override bool IsVirtualCurrency()
	{
		return false;
	}
}
