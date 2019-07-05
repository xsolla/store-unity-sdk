using Xsolla.Store;

public class CartItemModel
{
	public CartItemModel(StoreItem storeItem)
	{
		Sku = storeItem.sku;
		Price = storeItem.prices[0].amount;
		Currency = storeItem.prices[0].currency;
		Name = storeItem.name;
		ImgUrl = storeItem.image_url;
		Quantity = 1;
	}
	
	public string Sku { get; }
	public float Price { get; }
	public string Currency { get; }
	public string Name { get; }
	public string ImgUrl { get; }
	public int Quantity { get; set; }

	public float TotalPrice
	{
		get { return Price * Quantity; }
	}
}