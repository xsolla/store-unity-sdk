using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
    public class BattlePassExpiryTimeSetter : BaseBattlePassDescriptionSubscriber
    {
		[SerializeField] private Text ExpiryText;
		[SerializeField] private GameObject ExpiredLabel;

		private const string EXPIRY_TEXT_FORMAT = "Ends in {0}{1}";

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			if (!battlePassDescription.IsExpired)
			{
				SwapActive(ExpiryText.gameObject, ExpiredLabel);

				var expiryHours = GetExpirationHours(battlePassDescription.ExpiryDate);
				var expirationText = FormatExpirationText(expiryHours);

				ExpiryText.text = expirationText;
			}
			else
				SwapActive(ExpiredLabel, ExpiryText.gameObject);
		}

		private void SwapActive(GameObject setActive = null, GameObject setInactive = null)
		{
			setActive?.SetActive(true);
			setInactive?.SetActive(false);
		}

		private int GetExpirationHours(DateTime expirationDateTime)
		{
			return GetDifferenceInHours(expirationDateTime, DateTime.Now);
		}

		private int GetDifferenceInHours(DateTime dateTimeA, DateTime dateTimeB)
		{
			var timeSpan = dateTimeA - dateTimeB;
			var minutes = (int)timeSpan.TotalMinutes;
			minutes =  minutes > 0 ? minutes : -minutes;

			var hours = minutes / 60;
			var remainingMinutes = minutes - (hours * 60);
			
			if (remainingMinutes > 0)
				return hours + 1;
			else
				return hours;
		}

		private string FormatExpirationText(int hoursUntilExpiration)
		{
			var days = hoursUntilExpiration / 24;
			var hours = hoursUntilExpiration - (days * 24);

			var daysAsString = days > 0 ? $"{days}d " : string.Empty;
			var hoursAsString = $"{hours}h";
			
			return string.Format(EXPIRY_TEXT_FORMAT, daysAsString, hoursAsString);
		}
	}
}
