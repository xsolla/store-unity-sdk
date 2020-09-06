using Xsolla.Core;

public class UserCartItem
{
	public CatalogItemModel Item { get; }
	public int Quantity { get; set; }

	public string Sku => Item.Sku;
	public float  Price => Item.Price.HasValue ? Item.Price.Value.Value : 0F;
	public float  PriceWithoutDiscount => Item.RealPriceWithoutDiscount.HasValue ? Item.RealPriceWithoutDiscount.Value.Value : 0F;
	public string Currency => Item.Price?.Key;
	public string ImageUrl => Item.ImageUrl;
	public float  TotalPrice => PriceWithoutDiscount * Quantity;
	
	public UserCartItem(CatalogItemModel storeItem)
	{
		Item = storeItem;
		Quantity = 1;
	}
	
	/// <summary>
	/// Software calculation of the discount for demo project;
	/// </summary>
	public float TotalDiscount
	{
		get
		{
			if (XsollaSettings.StoreProjectId != "44056")
				return 0.0f;
			
			float totalDiscount = 0.0f;
			
			//Discount based on item quantity
			if (IsInRange(Quantity, 2, 4))
				totalDiscount += TotalPrice * 0.1f;
			else if (IsInRange(Quantity, 5, 9))
				totalDiscount += TotalPrice * 0.25f;
			else if (IsInRange(Quantity, 10, int.MaxValue))
				totalDiscount += TotalPrice * 0.5f;

			//Discount based on single item discount
			if (PriceWithoutDiscount != 0F && PriceWithoutDiscount != Price)
			{
				var priceDifference = PriceWithoutDiscount - Price;
				totalDiscount += priceDifference * (float)Quantity;
			}

			return totalDiscount;
		}
	}

	private static bool IsInRange(int value, int minimum, int maximum)
	{
		return value >= minimum && value <= maximum;
	}

	public override bool Equals(object obj)
	{
		return obj is UserCartItem item && Sku.Equals(item.Sku);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}