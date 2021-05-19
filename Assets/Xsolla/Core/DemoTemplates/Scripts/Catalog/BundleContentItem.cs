public class BundleContentItem
{
	private readonly CatalogItemModel _item;
	private readonly int _quantity;

	public CatalogItemModel Item
	{
		get
		{
			return _item;
		}
	}

	public int Quantity
	{
		get
		{
			return _quantity;
		}
	}

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


	public BundleContentItem(CatalogItemModel storeItem, int quantity)
	{
		_item = storeItem;
		_quantity = quantity;
	}
}
