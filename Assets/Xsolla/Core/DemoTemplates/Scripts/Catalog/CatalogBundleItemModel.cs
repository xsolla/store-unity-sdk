using System.Collections.Generic;

public class CatalogBundleItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency() => false;
	public override bool IsSubscription() => false;
	public override bool IsBundle() => true;

	public KeyValuePair<string, float>? ContentRealPrice { get; set; }
	public KeyValuePair<string, float>? ContentRealPriceWithoutDiscount { get; set; }

	public KeyValuePair<string, uint>? ContentVirtualPrice { get; set; }
	public KeyValuePair<string, uint>? ContentVirtualPriceWithoutDiscount { get; set; }

	public List<CatalogItemModel> Content { get; set; }
}