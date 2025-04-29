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
		[SerializeField] private Text PriceText;
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

		private void DrawPrice(StoreItem item)
		{
			VirtualPriceIcon.gameObject.SetActive(item.virtual_prices != null && item.virtual_prices.Length > 0);

			if (item.price != null)
			{
				var price = item.price;
				PriceText.text = $"{price.amount} {price.currency}";
			}
			else if (item.virtual_prices != null)
			{
				var price = item.virtual_prices.FirstOrDefault(x => x.is_default);
				if (price == null)
					price = item.virtual_prices.First();

				PriceText.text = $"{price.amount}";

				ImageLoader.LoadSprite(
					price.image_url,
					sprite => VirtualPriceIcon.sprite = sprite);
			}
		}

		private void Purchase(StoreItem item)
		{
			XsollaCatalog.Purchase(
				item.sku,
				XsollaReadyToUseStore.RisePurchaseSuccess,
				XsollaReadyToUseStore.RisePurchaseError);
		}
	}
}