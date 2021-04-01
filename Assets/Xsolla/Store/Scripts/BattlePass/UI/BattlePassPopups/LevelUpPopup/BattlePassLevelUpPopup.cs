using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpPopup : BaseBattlePassBuyPopup
	{
		[SerializeField] private BattlePassLevelUpPopupItemsShowcase ItemsShowcase = default;
		[SerializeField] private UserPlusMinusCounter Counter = default;
		[SerializeField] private Text Message = default;

		private const int COUNTER_LOWER_LIMIT = 1;
		private const string MESSAGE_TEMPLATE = "Upgrading to level {0} will unlock the following rewards:";

		private int _userCurrentLevel;

		public event Action<int> UserInput;

		public void Initialize(int userCurrentLevel, int levelUpLimit)
		{
			_userCurrentLevel = userCurrentLevel;

			Counter.Initialize(
				initialValue: COUNTER_LOWER_LIMIT,
				lowerLimit: COUNTER_LOWER_LIMIT,
				upperLimit: levelUpLimit);
			Counter.CounterChanged += OnCounterChange;
			OnCounterChange(COUNTER_LOWER_LIMIT);
		}

		public override void ShowPrice(string currencyImageUrl, string currencyName, int price, int userCurrency)
		{
			base.ShowPrice(currencyImageUrl, currencyName, price, userCurrency);
			Counter.ShowCounter(false);
		}

		public override void ShowPrice(string formattedPrice)
		{
			base.ShowPrice(formattedPrice);
			Counter.ShowCounter(true);
		}

		public void ShowItems(BattlePassItemDescription[] items)
		{
			ItemsShowcase.ShowItems(items);
		}

		private void OnCounterChange(int newCounterValue)
		{
			Message.text = string.Format(MESSAGE_TEMPLATE, _userCurrentLevel + newCounterValue);
			UserInput?.Invoke(newCounterValue);
		}
	}
}
