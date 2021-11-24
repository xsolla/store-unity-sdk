namespace Xsolla.Demo
{
	public class BundleContentItem
	{
		public CatalogItemModel Item { get; private set; }

		public string Sku
		{
			get
			{
				return Item.Sku;
			}
		}
		public string Name
		{
			get
			{
				return Item.Name;
			}
		}
		public string Description
		{
			get
			{
				return Item.Description;
			}
		}
		public string ImageUrl
		{
			get
			{
				return Item.ImageUrl;
			}
		}

		public int Quantity { get; private set; }

		public BundleContentItem(CatalogItemModel storeItem, int quantity)
		{
			Item = storeItem;
			Quantity = quantity;
		}
	}
}
