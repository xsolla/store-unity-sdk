using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;

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
	[SerializeField] GameObject expirationTimeObject;
	[SerializeField] Text expirationTimeText;
	[SerializeField] SimpleTextButton buyButton;
	[SerializeField] SimpleTextButton previewButton;
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

		if (!virtualItem.IsBundle())
			AttachBuyButtonHandler(virtualItem);
		else
		{
			DisableBuy();
			AttachPreviewButtonHandler(virtualItem);
		}

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

	private void DisableBuy()
	{
		buyButton.gameObject.SetActive(false);
		previewButton.gameObject.SetActive(true);
	}

	private void EnablePrice(bool isVirtualPrice)
	{
		itemPrice.gameObject.SetActive(!isVirtualPrice);
		itemPriceWithoutDiscount.gameObject.SetActive(false);
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
			if (!string.IsNullOrEmpty(currency.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(currency.ImageUrl, (_, sprite) => itemPriceVcImage.sprite = sprite);
			}
			else
			{
				Debug.LogError($"Virtual currency item with sku = '{virtualItem.Sku}' without image!");
			}
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
			if (isSelected)
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
		var currency = valuePair.Key;
		var price = valuePair.Value;
		itemPrice.text = PriceFormatter.FormatPrice(currency, price);

		var priceWithoutDiscountContainer = virtualItem.RealPriceWithoutDiscount;

		if (priceWithoutDiscountContainer == null || !priceWithoutDiscountContainer.HasValue || priceWithoutDiscountContainer.Value.Value == default(float))
			return;

		var priceWithoutDiscount = priceWithoutDiscountContainer.Value.Value;

		if (priceWithoutDiscount == price)
			return;
		else
		{
			itemPriceWithoutDiscount.text = PriceFormatter.FormatPrice(currency, priceWithoutDiscount);
			itemPriceWithoutDiscount.gameObject.SetActive(true);
		}
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
		if (!string.IsNullOrEmpty(virtualItem.ImageUrl))
		{
			ImageLoader.Instance.GetImageAsync(virtualItem.ImageUrl, LoadImageCallback);
		}
		else
		{
			Debug.LogError($"Virtual item item with sku = '{virtualItem.Sku}' without image!");
		}
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
		if (!virtualItem.IsSubscription()) return;
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

	private void AttachPreviewButtonHandler(CatalogItemModel virtualItem)
	{
		previewButton.onClick = () => { PopupFactory.Instance.CreateBundlePreview().SetBundleInfo((CatalogBundleItemModel) virtualItem); };
	}
}