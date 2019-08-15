using System;
using System.Linq;
using UnityEngine.SocialPlatforms;
using Xsolla.Core;
using Xsolla.Store;

public class CartItemModel
{
	public CartItemModel(StoreItem storeItem)
	{
		Sku = storeItem.sku;
		
		Price = storeItem.price.amount;
		Currency = storeItem.price.currency;
		
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
			
			if (IsInRange(Quantity, 10, Int32.MaxValue))
			{
				return Price * Quantity * 0.5f;
			}

			return 0.0f;
		}
	}
	
	static bool IsInRange(int value, int minimum, int maximum)
	{
		return value >= minimum && value <= maximum;
	}
}