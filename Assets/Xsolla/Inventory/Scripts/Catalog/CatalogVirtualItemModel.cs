namespace Xsolla.Demo
{
	public class CatalogVirtualItemModel : CatalogItemModel
	{
		public override bool IsVirtualCurrency() { return false; }
		public override bool IsSubscription() { return false; }
		public override bool IsBundle() { return false; }
	}
}
