using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class CartModel
{
	Dictionary<string, CartItemModel> cartItems = new Dictionary<string, CartItemModel>();

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

	public void RemoveCartItem(StoreItem item)
	{
		if (cartItems.ContainsKey(item.sku))
		{
			cartItems.Remove(item.sku);
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

	public Dictionary<string, CartItemModel> CartItems
	{
		get { return cartItems; }
	}
}