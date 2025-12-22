using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class StoreItemWidget : MonoBehaviour
	{
		[field: SerializeField] private TMP_Text NameText { get; set; }
		[field: SerializeField] private Image IconImage { get; set; }
		[field: SerializeField] private Button PurchaseButton { get; set; }
		[field: SerializeField] private GameObject FiatPriceContainer { get; set; }
		[field: SerializeField] private TMP_Text FiatPriceText { get; set; }
		[field: SerializeField] private GameObject VirtualPriceContainer { get; set; }
		[field: SerializeField] private Image VirtualCurrencyIconImage { get; set; }
		[field: SerializeField] private TMP_Text VirtualPriceText { get; set; }

		private IStoreService StoreService => ServiceLocator.Resolve<IStoreService>();
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private LocalizationService LocalizationService => ServiceLocator.Resolve<LocalizationService>();

		public void Setup(CatalogRecord record)
		{
			NameText.text = record.GetName();
			IconImage.sprite = record.IconSprite;

			PurchaseButton.onClick.AddListener(() => PurchaseItem(record));

			var isVirtualPurchase = record.IsPurchaseByVirtualCurrency();
			FiatPriceContainer.SetActive(!isVirtualPurchase);
			VirtualPriceContainer.SetActive(isVirtualPurchase);

			if (record.IsPurchaseByVirtualCurrency())
			{
				VirtualCurrencyIconImage.sprite = record.GetVirtualCurrencyIconSprite();
				VirtualPriceText.text = record.GetVirtualCurrencyAmount();
			}
			else
			{
				var currency = record.GetFiatCurrency();
				var amount = record.GetFiatPurchasePrice();
				FiatPriceText.text = $"{amount} {currency}";
			}
		}

		private void PurchaseItem(CatalogRecord record)
		{
			var itemSku = record.Sku;
			if (record.IsPurchaseByVirtualCurrency())
			{
				var priceSku = record.GetVirtualCurrencySku();
				StoreService.PurchaseForVirtualCurrency(itemSku, priceSku, null, OnPurchaseError);
			}
			else
			{
				StoreService.PurchaseItem(itemSku, null, OnPurchaseError);
			}
		}

		private void OnPurchaseError(string message)
		{
			message = GetErrorMessage(message);

			var popup = ScreenService
				.OpenInfoPopup()
				.SetTitle("Purchase Error")
				.SetMessage(message);

			popup.SetCloseCallback(() => ScreenService.Close(popup));
		}

		private string GetErrorMessage(string message)
		{
			if (message.Contains("[0401-1460]"))
				return LocalizationService.GetPurchaseLimitReachedMessage();
			
			if (message.Contains("[0401-5006]"))
				return LocalizationService.GetNotEnoughVirtualCurrencyMessage();

			return message;
		}
	}
}