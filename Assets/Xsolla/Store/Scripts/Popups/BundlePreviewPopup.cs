using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

namespace Xsolla.Core.Popup
{
	public class BundlePreviewPopup : MonoBehaviour, IBundlePreviewPopup
	{
		[SerializeField] Image bundleImage;
		[SerializeField] GameObject loadingCircle;
		[SerializeField] Text bundleName;
		[SerializeField] Text bundleDescription;
		[SerializeField] Text bundleInfo;
		[SerializeField] Text bundlePrice;
		[SerializeField] Text bundlePriceWithoutDiscount;
		[SerializeField] Image bundlePriceVcImage;
		[SerializeField] Text bundlePriceVc;
		[SerializeField] Image bundlePriceVcWithoutDiscountImage;
		[SerializeField] Text bundlePriceVcWithoutDiscount;

		[SerializeField] SimpleButton closeButton;
		[SerializeField] SimpleTextButton buyButton;

		[SerializeField] GameObject itemPrefab;
		[SerializeField] ItemContainer itemsContainer;

		public IBundlePreviewPopup SetBundleInfo(CatalogBundleItemModel bundle)
		{
			bundleName.text = bundle.Name;

			if(bundleDescription != null)
				bundleDescription.text = bundle.Description;

			bundleInfo.text = string.Format("This bundle includes '{0}' item{1}:", bundle.Content.Count, (bundle.Content.Count > 1 ? "s" : ""));

			if (!string.IsNullOrEmpty(bundle.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(bundle.ImageUrl, LoadImageCallback);
			}
			else
			{
				Debug.LogError(string.Format("Bundle with sku = '{0}' has no image!", bundle.Sku));
			}

			if (bundle.VirtualPrice != null)
				InitializeVirtualCurrencyPrice(bundle);
			else
				InitializeRealPrice(bundle);

			bundle.Content.ForEach(AddBundleContentItem);

			closeButton.onClick = () => { Destroy(gameObject, 0.001F); };

			AttachBuyButtonHandler(bundle);

			return this;
		}

		private void EnableRealPrice(bool bEnable)
		{
			bundlePrice.gameObject.SetActive(bEnable);
			bundlePriceWithoutDiscount.gameObject.SetActive(bEnable);
		}

		private void EnableVirtualCurrencyPrice(bool bEnable)
		{
			bundlePriceVcImage.gameObject.SetActive(bEnable);
			bundlePriceVc.gameObject.SetActive(bEnable);
			bundlePriceVcWithoutDiscountImage.gameObject.SetActive(bEnable);
			bundlePriceVcWithoutDiscount.gameObject.SetActive(bEnable);
		}

		private void InitializeVirtualCurrencyPrice(CatalogBundleItemModel bundle)
		{
			EnableRealPrice(false);
			EnableVirtualCurrencyPrice(true);

			bundlePriceVc.text = bundle.VirtualPrice.HasValue ? bundle.VirtualPrice.Value.ToString() : "";
			bundlePriceVcWithoutDiscount.text = bundle.ContentVirtualPriceWithoutDiscount.HasValue ? bundle.ContentVirtualPriceWithoutDiscount.Value.ToString() : "";

			if (!bundle.ContentVirtualPriceWithoutDiscount.HasValue)
			{
				bundlePriceVcWithoutDiscountImage.gameObject.SetActive(false);
				bundlePriceVcWithoutDiscount.gameObject.SetActive(false);
			}

			InitializeVcImages(bundle);
		}

		private void InitializeRealPrice(CatalogBundleItemModel bundle)
		{
			EnableRealPrice(true);
			EnableVirtualCurrencyPrice(false);

			var realPrice = bundle.RealPrice;
			if (realPrice == null)
			{
				Debug.LogError(string.Format("Bundle with sku = {0} has no price!", bundle.Sku));
				return;
			}

			var valuePair = realPrice.Value;
			var currency = valuePair.Key;
			var price = valuePair.Value;

			bundlePrice.text = PriceFormatter.FormatPrice(currency, price);

			var contentRealPrice = bundle.ContentRealPrice;
			if (contentRealPrice == null)
			{
				Debug.LogError(string.Format("Bundle with sku = {0} has no content price!", bundle.Sku));
				return;
			}

			var contentValuePair = contentRealPrice.Value;
			var contentCurrency = contentValuePair.Key;
			var contentPrice = contentValuePair.Value;

			bundlePriceWithoutDiscount.text = PriceFormatter.FormatPrice(contentCurrency, contentPrice);
		}

		private void LoadImageCallback(string url, Sprite sprite)
		{
			if (!bundleImage)
				return;
			
			loadingCircle.SetActive(false);
			bundleImage.gameObject.SetActive(true);
			bundleImage.sprite = sprite;
		}

		private void InitializeVcImages(CatalogBundleItemModel bundle)
		{
			var currencySku = default(string);
			if (bundle.VirtualPrice.HasValue)
				currencySku = bundle.VirtualPrice.Value.Key;

			var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Sku.Equals(currencySku));
			if (!string.IsNullOrEmpty(currency.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(currency.ImageUrl, (_, sprite) =>
				{
					if (bundlePriceVcImage)
					{
						bundlePriceVcImage.sprite = sprite;
						bundlePriceVcWithoutDiscountImage.sprite = sprite;
					}
				});
			}
			else
			{
				Debug.LogError(string.Format("Bundle with sku = '{0}' virtual currency price has no image!", bundle.Sku));
			}
		}

		private void AddBundleContentItem(BundleContentItem bundleContentItem)
		{
			itemsContainer.AddItem(itemPrefab).GetComponent<BundleItemUI>().Initialize(bundleContentItem);
		}

		private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
		{
			if (virtualItem.VirtualPrice == null)
			{
				buyButton.onClick = () =>
				{
					Destroy(gameObject, 0.001F);
					DemoShop.Instance.PurchaseForRealMoney(virtualItem);
				};
			}
			else
			{
				buyButton.onClick = () =>
				{
					Destroy(gameObject, 0.001F);
					DemoShop.Instance.PurchaseForVirtualCurrency(virtualItem);
				};
			}
		}
	}
}
