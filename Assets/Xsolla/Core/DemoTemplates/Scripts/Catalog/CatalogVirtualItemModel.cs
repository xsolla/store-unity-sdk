public class CatalogVirtualItemModel : CatalogItemModel
{
	public override bool IsVirtualCurrency() => false;
	public override bool IsSubscription() => false;
}