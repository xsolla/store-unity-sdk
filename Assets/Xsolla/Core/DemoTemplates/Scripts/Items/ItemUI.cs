using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class ItemUI : MonoBehaviour
{
	[SerializeField] Image itemImage;
	[SerializeField] GameObject loadingCircle;
	[SerializeField] Text itemName;
	[SerializeField] Text itemDescription;
	[SerializeField] Text itemPrice;
	[SerializeField] Text itemPriceWithoutDiscount;
	[SerializeField] Image itemPriceVcImage;
	[SerializeField] Text itemPriceVcText;
	[SerializeField] SimpleTextButton buyButton;

	private IDemoImplementation _demoImplementation;

	private void Awake()
	{
		itemPrice.gameObject.SetActive(false);
		itemPriceWithoutDiscount.gameObject.SetActive(false);
		itemPriceVcImage.gameObject.SetActive(false);
		itemPriceVcText.gameObject.SetActive(false);
	}

	public void Initialize(CatalogItemModel virtualItem, IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		
		if (virtualItem.VirtualPrice != null)
			InitializeVirtualCurrencyPrice(virtualItem);
		else
			InitializeRealPrice(virtualItem);

		InitializeVirtualItem(virtualItem);
		AttachBuyButtonHandler(virtualItem);
	}

	private void EnablePrice(bool isVirtualPrice)
	{
		itemPrice.gameObject.SetActive(!isVirtualPrice);
		itemPriceWithoutDiscount.gameObject.SetActive(!isVirtualPrice);
		itemPriceVcImage.gameObject.SetActive(isVirtualPrice);
		itemPriceVcText.gameObject.SetActive(isVirtualPrice);
	}

	private void InitializeVirtualCurrencyPrice(CatalogItemModel virtualItem)
	{
		EnablePrice(true);
		itemPriceVcText.text = virtualItem.VirtualPrice?.Value.ToString();
		if (UserInventory.Instance.Balance.Any(b => b.Sku.Equals(virtualItem.VirtualPrice?.Key)))
		{
			ImageLoader.Instance.GetImageAsync(
				UserInventory.Instance.Balance.First(b => b.Sku.Equals(virtualItem.VirtualPrice?.Key)).ImageUrl,
				(_, sprite) => itemPriceVcImage.sprite = sprite);
		}
	}

	private void InitializeRealPrice(CatalogItemModel virtualItem)
	{
		EnablePrice(false);
		var realPrice = virtualItem.RealPrice;
		if (realPrice == null)
		{
			Debug.LogError($"Catalog item with sku = {virtualItem.Sku} have not any price!");
			return;
		}
		var valuePair = realPrice.Value;
		var currency = RegionalCurrency.GetCurrencySymbol(valuePair.Key);
		var price = valuePair.Value.ToString("F2");
		itemPrice.text = FormatPriceText(currency, price);
		itemPriceWithoutDiscount.text = FormatDiscountPriceText(currency, price);
	}

	private void InitializeVirtualItem(CatalogItemModel virtualItem)
	{
		itemName.text = virtualItem.Name;
		itemDescription.text = virtualItem.Description;
		gameObject.name = "Item_" + virtualItem.Name.Replace(" ", "");
		ImageLoader.Instance.GetImageAsync(virtualItem.ImageUrl, LoadImageCallback);
	}
	
	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;
	}

	private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
	{
		if (virtualItem.VirtualPrice == null)
			buyButton.onClick = () => _demoImplementation.PurchaseForRealMoney(virtualItem);
		else
			buyButton.onClick = () => _demoImplementation.PurchaseForVirtualCurrency(virtualItem);
	}

	private static string FormatPriceText(string currency, string price)
	{
		return $"{currency}{price}";
	}

	private static string FormatDiscountPriceText(string currency, string price)
	{
		var result = "";
		FormatPriceText(currency, price).ToCharArray().ToList().ForEach(c => result += c + '\u0336');
		return result;
	}
}