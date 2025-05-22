using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreItemWidget : MonoBehaviour, IStoreItemWidget
	{
		[SerializeField] private Text NameText;
		[SerializeField] private Text DescriptionText;
		[SerializeField] private Image IconImage;
		[SerializeField] private Button BuyButton;
		[Space]
		[SerializeField] private GameObject FiatPriceContainer;
		[SerializeField] private GameObject VirtualPriceContainer;
		[SerializeField] private Text FiatPriceLabel;
		[SerializeField] private Text VirtualPriceLabel;
		[SerializeField] private Image VirtualPriceIcon;

		public void SetItem(StoreItem item)
		{
			NameText.text = item.name;
			DescriptionText.text = item.description;

			SpriteCache.Get(
				item.image_url,
				sprite => IconImage.sprite = sprite);

			DrawPrice(item);

			BuyButton.onClick.AddListener(() => Purchase(item));
		}

		private void OnDestroy()
		{
			BuyButton.onClick.RemoveAllListeners();
		}

		private void DrawPrice(StoreItem item)
		{
			FiatPriceContainer.SetActive(false);
			VirtualPriceContainer.SetActive(false);

			if (item.price != null)
			{
				FiatPriceContainer.SetActive(true);

				var price = GetFiatPrice(item);
				FiatPriceLabel.text = $"{price.amount} {price.currency}";
			}
			else if (item.virtual_prices != null)
			{
				VirtualPriceContainer.SetActive(true);

				var price = GetVirtualPrice(item);
				VirtualPriceLabel.text = $"{price.amount}";

				SpriteCache.Get(
					price.image_url,
					sprite => VirtualPriceIcon.sprite = sprite);
			}
		}

		private void Purchase(StoreItem item)
		{
			var isFiat = item.price != null;
			if (isFiat)
				PurchaseForFiat(item);
			else
				PurchaseForVirtualCurrency(item);
		}

		private Price GetFiatPrice(StoreItem item)
		{
			return item.price;
		}

		private VirtualPrice GetVirtualPrice(StoreItem item)
		{
			var price = item.virtual_prices.FirstOrDefault(x => x.is_default);
			if (price == null)
				price = item.virtual_prices.First();

			return price;
		}

		private void PurchaseForVirtualCurrency(StoreItem item)
		{
			var price = GetVirtualPrice(item);

			XsollaCatalog.PurchaseForVirtualCurrency(
				item.sku,
				price.sku,
				XsollaReadyToUseStore.RisePurchaseSuccess,
				XsollaReadyToUseStore.RisePurchaseError,
				sdkType: SdkType.ReadyToUseStore);
		}

		private void PurchaseForFiat(StoreItem item)
		{
			XsollaCatalog.Purchase(
				item.sku,
				XsollaReadyToUseStore.RisePurchaseSuccess,
				XsollaReadyToUseStore.RisePurchaseError,
				sdkType: SdkType.ReadyToUseStore);
		}
	}
}