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

	StoreController storeController;
	Coroutine _loadingRoutine;
	
	CartItemModel _itemInformation;

	void Awake()
	{
		storeController = FindObjectOfType<StoreController>();
		var cartItemContainer = FindObjectOfType<CartItemContainer>();
		var cartGroup = FindObjectOfType<CartGroupUI>();

		addButton.onClick = (() =>
		{
			storeController.CartModel.IncrementCartItem(_itemInformation.Sku);
			cartItemContainer.Refresh();
		});
		
		delButton.onClick = (() =>
		{
			if (_itemInformation.Quantity - 1 <= 0)
			{
				cartGroup.DecreaseCounter(_itemInformation.Quantity);
			}

			storeController.CartModel.DecrementCartItem(_itemInformation.Sku);
			cartItemContainer.Refresh();
		});
	}

	public void Initialize(CartItemModel itemInformation)
	{
		_itemInformation = itemInformation;

		itemPrice.text = FormatPriceText(_itemInformation.Currency, _itemInformation.Price);
		itemName.text = _itemInformation.Name;
		itemQuantity.text = _itemInformation.Quantity.ToString();
		
		if (_loadingRoutine == null && itemImage.sprite == null && _itemInformation.ImgUrl != "")
		{
			ImageLoader.Instance.GetImageAsync(_itemInformation.ImgUrl, LoadImageCallback);
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