using System.Collections.Generic;
using Xsolla.Store;

public class CartModel
{
	Dictionary<string, CartItemModel> cartItems = new Dictionary<string, CartItemModel>();
	
	public Dictionary<string, CartItemModel> CartItems
	{
		get { return cartItems; }
	}

	public void AddCartItem(StoreItem item)
	{
		if (!cartItems.ContainsKey(item.sku))
		{
			cartItems.Add(item.sku, new CartItemModel(item));
		}
	}
	public void IncrementCartItem(string itemSku)
	{
		if (cartItems.ContainsKey(itemSku))
		{
			cartItems[itemSku].Quantity++;
		}
	}

	public void RemoveCartItem(string itemSku)
	{
		if (cartItems.ContainsKey(itemSku))
		{
			cartItems.Remove(itemSku);
		}
	}

	public void DecrementCartItem(string itemSku)
	{
		if (cartItems.ContainsKey(itemSku))
		{
			if (cartItems[itemSku].Quantity <= 1)
			{
				cartItems.Remove(itemSku);
			}
			else
			{
				cartItems[itemSku].Quantity--;
			}
		}
	}
	
	public float CalculateCartPrice()
	{
		float cartPrice = 0.0f;

		foreach (var item in cartItems)
		{
			cartPrice += item.Value.TotalPrice;
		}
		
		return cartPrice;
	}

	public float CalculateCartDiscount()
	{
		float cartDiscount = 0.0f;

		foreach (var item in cartItems)
		{
			cartDiscount += item.Value.TotalDiscount;
		}
		
		return cartDiscount;
	}
}