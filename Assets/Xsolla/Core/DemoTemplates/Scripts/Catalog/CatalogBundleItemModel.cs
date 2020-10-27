using System.Collections.Generic;

public class CatalogBundleItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency() => false;
	public override bool IsSubscription() => false;
	public override bool IsBundle() => true;
	
	public List<CatalogItemModel> Content { get; set; }
}