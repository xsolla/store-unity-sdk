using System;
using System.Collections.Generic;
using UnityEngine;

public class CartItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject cartItemPrefab;

	[SerializeField]
	GameObject cartControlsPrefab;
	
	[SerializeField]
	Transform itemParent;

	private IDemoImplementation _demoImplementation;

	private readonly List<GameObject> _cartItems = new List<GameObject>();
	private GameObject _discountPanel;
	private GameObject _cartControls;
	
	private void Start()
	{
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

	public void SetStoreImplementation(IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
	}

	public void Refresh()
	{
		ClearContainerItems();
		
		if(UserCart.Instance.IsEmpty()) return;
		UserCart.Instance.GetItems().ForEach(AddCartItem);
		
		var fullPrice = UserCart.Instance.CalculateFullPrice();
		var discount = UserCart.Instance.CalculateCartDiscount();
		AddControls(fullPrice - discount);
	}

	private void AddCartItem(UserCartItem cartItem)
	{
		GameObject newItem = Instantiate(cartItemPrefab, itemParent);
		newItem.GetComponent<CartItemUI>().Initialize(cartItem);
		_cartItems.Add(newItem);
	}

	private void AddControls(float price)
	{
		_cartControls = Instantiate(cartControlsPrefab, itemParent);
		var controls = _cartControls.GetComponent<CartControls>();
		var discount = UserCart.Instance.CalculateCartDiscount();
		controls.Initialize(price, discount);
		controls.OnBuyCart = OnBuyCart;
		controls.OnClearCart = OnClearCart;
	}

	private void OnBuyCart()
	{
		var controls = _cartControls.GetComponent<CartControls>();
		if (controls.IsBuyButtonLocked()) return;
		controls.LockBuyButton();
		UserCart.Instance.Purchase(controls.UnlockBuyButton, _ => controls.UnlockBuyButton());
	}

	private void OnClearCart()
	{
		UserCart.Instance.Clear();
	}
}