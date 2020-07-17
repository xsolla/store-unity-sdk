using System.Collections;
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
	SimpleButton delButton;
	
	[SerializeField]
	Text itemQuantity;

	private UserCartItem _cartItem;

	void Awake()
	{
		addButton.onClick = () => UserCart.Instance.IncreaseCountOf(_cartItem.Item);
		delButton.onClick = () => UserCart.Instance.DecreaseCountOf(_cartItem.Item);
	}

	public void Initialize(UserCartItem cartItem)
	{
		_cartItem = cartItem;

		itemPrice.text = FormatPriceText(_cartItem.Currency, _cartItem.Price);
		itemName.text = _cartItem.Item.Name;
		itemQuantity.text = _cartItem.Quantity.ToString();
		
		if (itemImage.sprite == null && !string.IsNullOrEmpty(_cartItem.ImageUrl))
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
		if (string.IsNullOrEmpty(currencySymbol))
		{
			return string.Format("{0}{1}", currency, price);
		}
		
		return string.Format("{0}{1}", currencySymbol, price);
	}
}