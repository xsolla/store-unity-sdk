using System;
using UnityEngine;

namespace Xsolla.Demo
{
    public abstract class BaseBattlePassBuyPopup : MonoBehaviour
    {
		[SerializeField] private SimpleButton[] CloseButtons;
		[SerializeField] private BattlePassBuyBlock BuyBlock;

		public event Action BuyButtonClick;

		private void Awake()
		{
			foreach (var button in CloseButtons)
				button.onClick += ClosePopup;

			BuyBlock.BuyButtonClick += RaiseBuyButtonClick;
			BuyBlock.BuyCurrencyButtonClick += GotoCurrencyBuy;
		}

		public virtual void ShowPrice(string formattedPrice)
		{
			BuyBlock.ShowPrice(formattedPrice);
		}

		public virtual void ShowPrice(string currencyImageUrl, string currencyName, int price, int userCurrency)
		{
			BuyBlock.ShowPrice(currencyImageUrl, currencyName, price, userCurrency);
		}

		private void RaiseBuyButtonClick()
		{
			ClosePopup();
			if (BuyButtonClick != null)
				BuyButtonClick.Invoke();
		}

		private void GotoCurrencyBuy()
		{
			ClosePopup();
			DemoController.Instance.SetState(MenuState.BuyCurrency);
		}

		private void ClosePopup()
		{
			Destroy(this.gameObject);
		}
	}
}
