using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla;

public class CartItemContainer : MonoBehaviour
{
	[SerializeField]
	GameObject _cart_item_Prefab;

	[SerializeField]
	Transform _item_Parent;

	List<GameObject> cartItems;

	void Awake()
	{
		cartItems = new List<GameObject>();
	}

	public void AddCartItem(CartItem itemInformation)
	{
		GameObject newItem = Instantiate(_cart_item_Prefab, _item_Parent);
		cartItems.Add(newItem);
		newItem.GetComponent<CartItemUI>().Initialize(itemInformation);
	}

	public void ClearCartItems()
	{
		foreach (var item in cartItems)
		{
			Destroy(item);
		}
		
		cartItems.Clear();
	}
}