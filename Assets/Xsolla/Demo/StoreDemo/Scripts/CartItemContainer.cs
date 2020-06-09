using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public class CartItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject cartItemPrefab;

	[SerializeField]
	GameObject cartDiscountPrefab;
	
	[SerializeField]
	GameObject cartControlsPrefab;
	
	[SerializeField]
	Transform itemParent;

	private List<GameObject> _cartItems;
	private GameObject _discountPanel;
	private GameObject _cartControls;
	
	void Awake()
	{
		_cartItems = new List<GameObject>();

		UserCart.Instance.ClearCartEvent += ClearContainerItems;
		UserCart.Instance.AddItemEvent += (_) => Refresh();
		UserCart.Instance.RemoveItemEvent += (_) => Refresh();
		UserCart.Instance.UpdateItemEvent += (item, deltaQuantity) => Refresh();
	}
	
	private void ClearContainerItems()
	{
		_cartItems.ForEach(Destroy);
		_cartItems.Clear();
		
		Destroy(_discountPanel);
		Destroy(_cartControls);
	}

	public void Refresh()
	{
		ClearContainerItems();
		
		if(UserCart.Instance.IsEmpty()) return;
		UserCart.Instance.GetCartItems().ForEach(AddCartItem);
		
		var discount = UserCart.Instance.CalculateCartDiscount();
		if (discount > 0.0f)
		{
			AddDiscount(discount);
		}
		var fullPrice = UserCart.Instance.CalculateFullPrice();
		AddControls(fullPrice - discount);
	}

	private void AddCartItem(UserCartItem cartItem)
	{
		GameObject newItem = Instantiate(cartItemPrefab, itemParent);
		newItem.GetComponent<CartItemUI>().Initialize(cartItem);
		_cartItems.Add(newItem);
	}

	private void AddDiscount(float discountAmount)
	{
		_discountPanel = Instantiate(cartDiscountPrefab, itemParent);
		_discountPanel.GetComponent<CartDiscountUI>().Initialize(discountAmount);
	}

	private void AddControls(float price)
	{
		_cartControls = Instantiate(cartControlsPrefab, itemParent);
		CartControls controls = _cartControls.GetComponent<CartControls>();
		controls.Initialize(price);
		controls.OnBuyCart = OnBuyCart;
		controls.OnClearCart = OnClearCart;
	}

	private void OnBuyCart()
	{
		CartControls controls = _cartControls.GetComponent<CartControls>();
		if (controls.IsBuyButtonLocked()) return;
		controls.LockBuyButton();
		UserCart.Instance.Purchase(controls.UnlockBuyButton, _ => controls.UnlockBuyButton());
	}

	private void OnClearCart()
	{
		UserCart.Instance.Clear();
	}
}