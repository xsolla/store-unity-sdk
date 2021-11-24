using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BattlePassBuyBlock : MonoBehaviour
    {
		[SerializeField] private SimpleTextButtonDisableable BuyButton;
		[SerializeField] private SimpleButton BuyCurrencyButton;
		[Space]
		[SerializeField] private Text RealCurrencyPriceValue;
		[Space]
		[SerializeField] private Image VirtualCurrencyIcon;
		[SerializeField] private Text VirtualCurrencyPriceValue;
		[Space]
		[SerializeField] private GameObject BuyCurrencyButtonHolder;
		[SerializeField] private Text CurrencyDeltaText;

		private const string CURRENCY_DELTA_TEMPLATE = "You need {0} more {1}.";
		private const string CURRENCY_GOLD = "GOLD";

		public event Action BuyButtonClick;
		public event Action BuyCurrencyButtonClick;

		private void Awake()
		{
			BuyButton.onClick += () =>
			{
				if (BuyButtonClick != null)
					BuyButtonClick.Invoke();
			};
			BuyCurrencyButton.onClick += () =>
			{
				if (BuyCurrencyButtonClick != null)
					BuyCurrencyButtonClick.Invoke();
			};
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
				var currencyNameToShow = (currencyName.EndsWith("s") || currencyName.ToUpper() == CURRENCY_GOLD) ? currencyName : string.Format("{0}s", currencyName);

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
