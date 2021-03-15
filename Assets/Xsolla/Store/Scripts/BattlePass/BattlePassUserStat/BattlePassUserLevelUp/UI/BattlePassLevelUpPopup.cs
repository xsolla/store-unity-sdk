using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpPopup : MonoBehaviour
    {
		[SerializeField] private SimpleButton[] CloseButtons = default;
		[SerializeField] private BattlePassLevelUpPopupItemsShowcase ItemsShowcase = default;
		[SerializeField] private UserPlusMinusCounter Counter = default;
		[SerializeField] private BattlePassLevelUpBuyBlock BuyBlock = default;

		private const int COUNTER_LOWER_LIMIT = 1;

		public event Action<int> UserInput;
		public event Action BuyButtonClick;

		private void Awake()
		{
			foreach (var button in CloseButtons)
				button.onClick += ClosePopup;

			BuyBlock.BuyButtonClick += RaiseBuyButtonClick;
			BuyBlock.BuyCurrencyButtonClick += GotoCurrencyBuy;
		}

		public void Initialize(int counterLimit)
		{
			Counter.Initialize(
				initialValue: COUNTER_LOWER_LIMIT,
				lowerLimit: COUNTER_LOWER_LIMIT,
				upperLimit: counterLimit);
			Counter.CounterChanged += OnCounterChange;
			OnCounterChange(COUNTER_LOWER_LIMIT);
		}

		public void ShowItems(BattlePassItemDescription[] items)
		{
			ItemsShowcase.ShowItems(items);
		}

		public void ShowPrice(string formattedPrice) => BuyBlock.ShowPrice(formattedPrice);
		public void ShowPrice(string currencyImageUrl, int price, int userCurrency) => BuyBlock.ShowPrice(currencyImageUrl, price, userCurrency);

		private void OnCounterChange(int newCounterValue)
		{
			UserInput?.Invoke(newCounterValue);
		}

		private void RaiseBuyButtonClick()
		{
			ClosePopup();
			BuyButtonClick?.Invoke();
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
