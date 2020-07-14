using System.Collections.Generic;

public class CatalogItemModel : ItemModel
{
	public KeyValuePair<string, uint>? VirtualPrice { get; set; }
	public KeyValuePair<string, float>? RealPrice { get; set; }

	public override bool IsVirtualCurrency() => false;
}