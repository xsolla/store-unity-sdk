using System;
using Xsolla.Core;
using Xsolla.Store;

public class UserCartItem
{
	public StoreItem Item { get; }
	public int Quantity { get; set; }

	public string Sku => Item.sku;
	public float Price => Item.price.amount;
	public string Currency => Item.price.currency;
	public string ImageUrl => Item.image_url;
	public float TotalPrice => Price * Quantity;
	
	public UserCartItem(StoreItem storeItem)
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
			{
				return 0.0f;
			}
			
			if (IsInRange(Quantity, 2, 4))
			{
				return Price * Quantity * 0.1f;
			}
			
			if (IsInRange(Quantity, 5, 9))
			{
				return Price * Quantity * 0.25f;
			}
			
			if (IsInRange(Quantity, 10, int.MaxValue))
			{
				return Price * Quantity * 0.5f;
			}

			return 0.0f;
		}
	}

	private static bool IsInRange(int value, int minimum, int maximum)
	{
		return value >= minimum && value <= maximum;
	}
}