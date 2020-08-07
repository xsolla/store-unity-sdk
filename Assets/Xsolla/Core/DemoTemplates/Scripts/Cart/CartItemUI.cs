using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class CartItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemPrice;

	[SerializeField]
	SimpleButton addButton;
	[SerializeField]
	SimpleButton removeButton;
	[SerializeField]
	SimpleButton deleteButton;
	
	[SerializeField]
	Text itemQuantity;

	private UserCartItem _cartItem;

	private void Start()
	{
		addButton.onClick = () => UserCart.Instance.IncreaseCountOf(_cartItem.Item);
		removeButton.onClick = () => UserCart.Instance.DecreaseCountOf(_cartItem.Item);
		deleteButton.onClick = () => UserCart.Instance.RemoveItem(_cartItem.Item);

		UserCart.Instance.RemoveItemEvent += RemoveItemHandler;
		UserCart.Instance.UpdateItemEvent += UpdateItemHandler;
	}

	private void OnDestroy()
	{
		UserCart.Instance.RemoveItemEvent -= RemoveItemHandler;
		UserCart.Instance.UpdateItemEvent -= UpdateItemHandler;
	}

	private void RemoveItemHandler(UserCartItem item)
	{
		if (!item.Equals(_cartItem)) return;
		Destroy(gameObject, 0.1F);
	}

	private void UpdateItemHandler(UserCartItem item, int deltaCount)
	{
		if (!item.Equals(_cartItem)) return;
		itemQuantity.text = _cartItem.Quantity.ToString();
	}

	public void Initialize(UserCartItem cartItem)
	{
		_cartItem = cartItem;

		itemPrice.text = FormatPriceText(_cartItem.Currency, _cartItem.Price);
		itemName.text = _cartItem.Item.Name;
		itemQuantity.text = _cartItem.Quantity.ToString();
		
		if (itemImage.sprite != null && !string.IsNullOrEmpty(_cartItem.ImageUrl))
		{
			ImageLoader.Instance.GetImageAsync(_cartItem.ImageUrl, LoadImageCallback);
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		itemImage.sprite = image;
	}

	string FormatPriceText(string currency, float price)
	{
		var currencySymbol = RegionalCurrency.GetCurrencySymbol(currency);
		return string.IsNullOrEmpty(currencySymbol) 
			? $"{currency}{price}" 
			: $"{currencySymbol}{price}";
	}
}