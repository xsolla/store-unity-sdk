using System.Collections.Generic;

public class CatalogBundleItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency()
	{
		return false;
	}

	public override bool IsSubscription()
	{
		return false;
	}

	public override bool IsBundle()
	{
		return true;
	}

	public KeyValuePair<string, float>? ContentRealPrice { get; set; }
	public KeyValuePair<string, float>? ContentRealPriceWithoutDiscount { get; set; }

	public KeyValuePair<string, uint>? ContentVirtualPrice { get; set; }
	public KeyValuePair<string, uint>? ContentVirtualPriceWithoutDiscount { get; set; }

	public List<BundleContentItem> Content { get; set; }
}
