using Xsolla.Core;

public class UserCartItem
{
	private readonly CatalogItemModel _item;
	public CatalogItemModel Item
	{
		get
		{
			return _item;
		}
	}
	public int Quantity { get; set; }

	public string Sku
	{
		get
		{
			return Item.Sku;
		}
	}

	public float Price
	{
		get
		{
			return Item.Price.HasValue ? Item.Price.Value.Value : 0F;
		}
	}

	public float PriceWithoutDiscount
	{
		get
		{
			return Item.RealPriceWithoutDiscount.HasValue ? Item.RealPriceWithoutDiscount.Value.Value : 0F;
		}
	}

	public string Currency
	{
		get
		{
			return Item.Price.HasValue ? Item.Price.Value.Key : null;
		}
	}
		
	public string ImageUrl
	{
		get
		{
			return Item.ImageUrl;
		}
	}

	public float TotalPrice
	{
		get
		{
			return PriceWithoutDiscount * Quantity;
		}
	}
	
	public UserCartItem(CatalogItemModel storeItem)
	{
		_item = storeItem;
		Quantity = 1;
	}
	
	/// <summary>
	/// Software calculation of the discount for demo project;
	/// </summary>
	public float TotalDiscount
	{
		get
		{
			float totalDiscount = 0.0f;
			
			if (XsollaSettings.StoreProjectId == "44056")
			{
				//Cutom discount based on item quantity to match backend rules for this project
				if (IsInRange(Quantity, 2, 4))
					totalDiscount += (Price * Quantity) * 0.1f;
				else if (IsInRange(Quantity, 5, 9))
					totalDiscount += (Price * Quantity) * 0.25f;
				else if (IsInRange(Quantity, 10, int.MaxValue))
					totalDiscount += (Price * Quantity) * 0.5f;
			}

			//Discount based on single item discount
			if (PriceWithoutDiscount != Price)
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
		if (obj is UserCartItem)
		{
			var item = (UserCartItem)obj;
			return Sku.Equals(item.Sku);
		}
		else
			return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
