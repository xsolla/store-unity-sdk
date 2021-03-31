using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BattlePassBuyBlock : MonoBehaviour
    {
		[SerializeField] private SimpleTextButtonDisableable BuyButton = default;
		[SerializeField] private SimpleButton BuyCurrencyButton = default;
		[Space]
		[SerializeField] private Text RealCurrencyPriceValue = default;
		[Space]
		[SerializeField] private Image VirtualCurrencyIcon = default;
		[SerializeField] private Text VirtualCurrencyPriceValue = default;
		[Space]
		[SerializeField] private GameObject BuyCurrencyButtonHolder = default;
		[SerializeField] private Text CurrencyDeltaText = default;

		private const string CURRENCY_DELTA_TEMPLATE = "You need {0} more {1}.";
		private const string CURRENCY_GOLD = "GOLD";

		public event Action BuyButtonClick;
		public event Action BuyCurrencyButtonClick;

		private void Awake()
		{
			BuyButton.onClick += () => BuyButtonClick?.Invoke();
			BuyCurrencyButton.onClick += () => BuyCurrencyButtonClick?.Invoke();
		}

		public void ShowPrice(string formattedPrice)
		{
			SetActive(VirtualCurrencyIcon.gameObject, false);
			SetActive(VirtualCurrencyPriceValue.gameObject, false);
			SetActive(BuyCurrencyButtonHolder, false);
			SetActive(CurrencyDeltaText.gameObject, false);
			
			//Trim '.00' price ending
			if (formattedPrice.EndsWith(".00") || formattedPrice.EndsWith(",00"))
				formattedPrice = formattedPrice.Remove(formattedPrice.Length - 3, 3);

			RealCurrencyPriceValue.text = formattedPrice;
			SetActive(RealCurrencyPriceValue.gameObject, true);
		}

		public void ShowPrice(string currencyImageUrl, string currencyName, int price, int userCurrency)
		{
			SetActive(RealCurrencyPriceValue.gameObject, false);

			ImageLoader.Instance.GetImageAsync(currencyImageUrl, OnGetImage);

			VirtualCurrencyPriceValue.text = price.ToString();
			SetActive(VirtualCurrencyPriceValue.gameObject, true);

			var isEnoughCurrency = price <= userCurrency;
			if (isEnoughCurrency)
				BuyButton.Enable();
			else
			{
				BuyButton.Disable();
				var currencyDelta = price - userCurrency;
				var currencyNameToShow = (currencyName.EndsWith("s") || currencyName.ToUpper() == CURRENCY_GOLD) ? currencyName : $"{currencyName}s";

				var currencyDeltaMessage = string.Format(CURRENCY_DELTA_TEMPLATE, currencyDelta, currencyNameToShow);
				CurrencyDeltaText.text = currencyDeltaMessage;
			}

			SetActive(BuyCurrencyButtonHolder, !isEnoughCurrency);
			SetActive(CurrencyDeltaText.gameObject, !isEnoughCurrency);
		}

		private void SetActive(GameObject gameObject, bool targetValue)
		{
			if (gameObject.activeSelf != targetValue)
				gameObject.SetActive(targetValue);
		}

		private void OnGetImage(string _ , Sprite imageSprite)
		{
			if (VirtualCurrencyIcon)
			{
				VirtualCurrencyIcon.sprite = imageSprite;
				SetActive(VirtualCurrencyIcon.gameObject, true);
			}
		}
	}
}
