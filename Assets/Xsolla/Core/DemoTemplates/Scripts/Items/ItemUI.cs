using System;
using System.Collections;
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
	[SerializeField] Image itemPriceStrokeImage;
	[SerializeField] Image itemPriceVcImage;
	[SerializeField] Text itemPriceVcText;
	[SerializeField] GameObject expirationTimeObject;
	[SerializeField] Text expirationTimeText;
	[SerializeField] SimpleTextButton buyButton;
	[SerializeField] AddToCartButton cartButton;
	[SerializeField] GameObject prices;
	[SerializeField] GameObject purchasedText;

	private IDemoImplementation _demoImplementation;
	private CatalogItemModel _itemInformation;

	private void Awake()
	{
		itemPrice.gameObject.SetActive(false);
		itemPriceWithoutDiscount.gameObject.SetActive(false);
		itemPriceVcImage.gameObject.SetActive(false);
		itemPriceVcText.gameObject.SetActive(false);
		expirationTimeObject.SetActive(false);
	}

	public void Initialize(CatalogItemModel virtualItem, IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		_itemInformation = virtualItem;
		
		if (virtualItem.VirtualPrice != null)
			InitializeVirtualCurrencyPrice(virtualItem);
		else
			InitializeRealPrice(virtualItem);

		InitializeVirtualItem(virtualItem);
		AttachBuyButtonHandler(virtualItem);

		if (!virtualItem.IsConsumable)
		{
			var sameItemFromInventory = UserInventory.Instance.VirtualItems.FirstOrDefault(i => i.Sku.Equals(_itemInformation.Sku));
			var isAlreadyPurchased = sameItemFromInventory != null;
			
			if (isAlreadyPurchased)
				DisablePrice();
		}
	}

	private void DisablePrice()
	{
		prices.SetActive(false);
		purchasedText.SetActive(true);
	}

	private void EnablePrice(bool isVirtualPrice)
	{
		itemPrice.gameObject.SetActive(!isVirtualPrice);
		itemPriceWithoutDiscount.gameObject.SetActive(false);
		itemPriceStrokeImage.gameObject.SetActive(false);
		itemPriceVcImage.gameObject.SetActive(isVirtualPrice);
		itemPriceVcText.gameObject.SetActive(isVirtualPrice);
	}

	private void InitializeVirtualCurrencyPrice(CatalogItemModel virtualItem)
	{
		EnablePrice(true);
		cartButton.gameObject.SetActive(false);
		itemPriceVcText.text = virtualItem.VirtualPrice?.Value.ToString();
		InitializeVcImage(virtualItem);
	}

	private void InitializeVcImage(CatalogItemModel virtualItem)
	{
		StartCoroutine(WaitCatalogUpdate(() =>
		{
			var currencySku = virtualItem.VirtualPrice?.Key;
			var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Sku.Equals(currencySku));
			ImageLoader.Instance.GetImageAsync(currency.ImageUrl, (_, sprite) => itemPriceVcImage.sprite = sprite);
		}));
	}

	IEnumerator WaitCatalogUpdate(Action callback)
	{
		yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
		callback?.Invoke();
	}

	private void InitializeRealPrice(CatalogItemModel virtualItem)
	{
		EnablePrice(false);
		cartButton.gameObject.SetActive(true);
		if (UserCart.Instance.Contains(virtualItem.Sku))
			cartButton.Select(true);
		cartButton.onClick = isSelected =>
		{
			if(isSelected)
				UserCart.Instance.AddItem(_itemInformation);
			else
				UserCart.Instance.RemoveItem(_itemInformation);
		};
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
		itemPriceWithoutDiscount.text = FormatPriceText(currency, price);
	}

	private void InitializeVirtualItem(CatalogItemModel virtualItem)
	{
		if (string.IsNullOrEmpty(virtualItem.Name))
		{
			Debug.LogError($"Try initialize item with sku = {virtualItem.Sku} without name!");
			virtualItem.Name = virtualItem.Sku;
		}
		itemName.text = virtualItem.Name;
		itemDescription.text = virtualItem.Description;
		gameObject.name = "Item_" + virtualItem.Name.Replace(" ", "");
		ImageLoader.Instance.GetImageAsync(virtualItem.ImageUrl, LoadImageCallback);
	}
	
	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.gameObject.SetActive(true);
		itemImage.sprite = image;
		InitExpirationTime(_itemInformation);
	}
	
	private void InitExpirationTime(CatalogItemModel virtualItem)
	{
		expirationTimeObject.SetActive(false);
		if(!virtualItem.IsSubscription()) return;
		var subscription = UserCatalog.Instance.Subscriptions.First(s => s.Sku.Equals(virtualItem.Sku));
		if (subscription == null)
		{
			Debug.LogError($"Something went wrong... Can not find subscription item with sku = '{virtualItem.Sku}'!");
			return;
		}
		expirationTimeObject.SetActive(true);
		expirationTimeText.text = subscription.ExpirationPeriodText;
	}

	private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
	{
		if (virtualItem.VirtualPrice == null)
		{
			buyButton.onClick = () => _demoImplementation.PurchaseForRealMoney(virtualItem);
		}
		else
		{
			buyButton.onClick = () => _demoImplementation.PurchaseForVirtualCurrency(virtualItem);
		}
	}

	private static string FormatPriceText(string currency, string price)
	{
		return $"{currency}{price}";
	}
}