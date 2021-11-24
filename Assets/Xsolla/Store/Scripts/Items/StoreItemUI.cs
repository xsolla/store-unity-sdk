using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class StoreItemUI : MonoBehaviour
	{
		[SerializeField] Image itemImage;
		[SerializeField] GameObject loadingCircle;
		[SerializeField] Text itemName;
		[SerializeField] Text itemDescription;
		[SerializeField] int itemDescriptionLength;
		[SerializeField] Text itemPrice;
		[SerializeField] Text itemPriceWithoutDiscount;
		[SerializeField] Image itemPriceVcImage;
		[SerializeField] Text itemPriceVcText;
		[SerializeField] GameObject expirationTimeObject;
		[SerializeField] Text expirationTimeText;
		[SerializeField] SimpleTextButton buyButton;
		[SerializeField] SimpleTextButton previewButton;
		[SerializeField] AddToCartButton cartButton;
		[SerializeField] SimpleTextButton checkoutButtonButton;
		[SerializeField] GameObject prices;
		[SerializeField] GameObject purchasedText;

		private CatalogItemModel _itemInformation;

		public bool IsAlreadyPurchased { get; private set; }

		public event Action<CatalogItemModel> OnInitialized;

		private void Awake()
		{
			itemPrice.gameObject.SetActive(false);
			itemPriceWithoutDiscount.gameObject.SetActive(false);
			itemPriceVcImage.gameObject.SetActive(false);
			itemPriceVcText.gameObject.SetActive(false);
			expirationTimeObject.SetActive(false);
		}

		public void Initialize(CatalogItemModel virtualItem)
		{
			_itemInformation = virtualItem;

			if (virtualItem.VirtualPrice != null)
				InitializeVirtualCurrencyPrice(virtualItem);
			else
				InitializeRealPrice(virtualItem);

			InitializeVirtualItem(virtualItem);

			var isBundle = virtualItem.IsBundle();
			DisableBuy(isBundle);
			if (!isBundle)
				AttachBuyButtonHandler(virtualItem);
			else
				AttachPreviewButtonHandler(virtualItem);

			var isPurchased = CheckIfItemPurchased(virtualItem);
			DisablePrice(isPurchased);

			if (UserCart.Instance.Contains(virtualItem.Sku))
				EnableCheckout(true);

			checkoutButtonButton.onClick = () => DemoController.Instance.SetState(MenuState.Cart);

			if (OnInitialized != null)
				OnInitialized.Invoke(virtualItem);
		}

		private bool CheckIfItemPurchased(CatalogItemModel virtualItem)
		{
			if (!virtualItem.IsConsumable)
			{
				var sameItemFromInventory = UserInventory.Instance.VirtualItems.FirstOrDefault(i => i.Sku.Equals(_itemInformation.Sku));
				IsAlreadyPurchased = sameItemFromInventory != null;
				return IsAlreadyPurchased;
			}

			//else
			return false;
		}

		private void DisablePrice(bool disable)
		{
			prices.SetActive(!disable);
			purchasedText.SetActive(disable);
		}

		private void DisableBuy(bool disable)
		{
			buyButton.gameObject.SetActive(!disable);
			previewButton.gameObject.SetActive(disable);
		}

		private void EnablePrice(bool isVirtualPrice)
		{
			itemPrice.gameObject.SetActive(!isVirtualPrice);
			itemPriceWithoutDiscount.gameObject.SetActive(false);
			itemPriceVcImage.gameObject.SetActive(isVirtualPrice);
			itemPriceVcText.gameObject.SetActive(isVirtualPrice);
		}

		private void EnableCheckout(bool enableCheckout)
		{
			checkoutButtonButton.gameObject.SetActive(enableCheckout);
			previewButton.gameObject.SetActive(!enableCheckout);
			cartButton.gameObject.SetActive(!enableCheckout);
			buyButton.gameObject.SetActive(!enableCheckout);
		}

		private void InitializeVirtualCurrencyPrice(CatalogItemModel virtualItem)
		{
			EnablePrice(true);
			cartButton.gameObject.SetActive(false);

			if (virtualItem.VirtualPrice.HasValue)
				itemPriceVcText.text = virtualItem.VirtualPrice.Value.ToString();

			InitializeVcImage(virtualItem);
		}

		private void InitializeVcImage(CatalogItemModel virtualItem)
		{
			StartCoroutine(WaitCatalogUpdate(() =>
			{
				var currencySku = default(string);
				if (virtualItem.VirtualPrice.HasValue)
					currencySku = virtualItem.VirtualPrice.Value.Key;

				var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Sku.Equals(currencySku));

				if (!string.IsNullOrEmpty(currency.ImageUrl))
					ImageLoader.Instance.GetImageAsync(currency.ImageUrl, (_, sprite) =>
					{
						if (itemPriceVcImage)
							itemPriceVcImage.sprite = sprite;
					});
				else
					Debug.LogError(string.Format("Virtual currency item with sku = '{0}' without image!", virtualItem.Sku));
			}));
		}

		IEnumerator WaitCatalogUpdate(Action callback)
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			if (callback != null)
				callback.Invoke();
		}

		IEnumerator WaitInventoryUpdate(Action callback)
		{
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);
			if (callback != null)
				callback.Invoke();
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

				EnableCheckout(isSelected);
			};
			var realPrice = virtualItem.RealPrice;
			if (realPrice == null)
			{
				Debug.LogError(string.Format("Catalog item with sku = {0} have not any price!", virtualItem.Sku));
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
				Debug.LogError(string.Format("Try initialize item with sku = {0} without name!", virtualItem.Sku));
				virtualItem.Name = virtualItem.Sku;
			}

			itemName.text = virtualItem.Name;
			itemDescription.text = ShortenDescription(virtualItem.Description, itemDescriptionLength);
			gameObject.name = "Item_" + virtualItem.Name.Replace(" ", "");
			if (!string.IsNullOrEmpty(virtualItem.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(virtualItem.ImageUrl, LoadImageCallback);
			}
			else
			{
				Debug.LogError(string.Format("Virtual item item with sku = '{0}' without image!", virtualItem.Sku));
			}
		}

		private void LoadImageCallback(string url, Sprite image)
		{
			if (!itemImage)
				return;
			
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
				Debug.LogError(string.Format("Something went wrong... Can not find subscription item with sku = '{0}'!", virtualItem.Sku));
				return;
			}

			expirationTimeObject.SetActive(true);
			expirationTimeText.text = subscription.ExpirationPeriodText;
		}

		private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
		{
			Action<CatalogItemModel> onPurchased = item => { StartCoroutine(WaitInventoryUpdate(() => CheckIfItemPurchased(item))); };

			if (virtualItem.VirtualPrice == null)
			{
				buyButton.onClick = () => DemoShop.Instance.PurchaseForRealMoney(virtualItem, onPurchased);
			}
			else
			{
				buyButton.onClick = () => DemoShop.Instance.PurchaseForVirtualCurrency(virtualItem, onPurchased);
			}
		}

		private void AttachPreviewButtonHandler(CatalogItemModel virtualItem)
		{
			previewButton.onClick = () => { PopupFactory.Instance.CreateBundlePreview().SetBundleInfo((CatalogBundleItemModel) virtualItem); };
		}

		private string ShortenDescription(string input, int limit)
		{
			if (input.Length <= limit)
				return input;

			if (limit > input.Length - 1)
				limit = input.Length - 1;

			var spacePosition = input.LastIndexOf(" ", limit);
			var result = default(string);

			if (spacePosition != -1)
				result = input.Substring(0, spacePosition);
			else
				result = input.Substring(0, limit);

			return result;
		}
	}
}
