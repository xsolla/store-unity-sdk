using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassDescriptionConverter : BaseBattlePassDescriptionConverter
	{
		public override void OnBattlePassJsonExtracted(IEnumerable<BattlePassDescriptionRaw> battlePassJsonItems)
		{
			var convertedDescriptions = new List<BattlePassDescription>();

			foreach (var item in battlePassJsonItems)
			{
				var expiryDateObtained = ConvertDateTime(item.ExpiryDate, out DateTime expiryDate);

				if (!expiryDateObtained)
				{
					Debug.LogError($"Could not convert expiry date: '{item.ExpiryDate}'");
					continue;
				}

				var isExpired = DefineExpiration(expiryDate);
				var convertedLevels = ConvertLevels(item.Levels);

				var convertedDescription = new BattlePassDescription
				(
					name = item.Name,
					expiryDate: expiryDate,
					isExpired: isExpired,
					levels: convertedLevels
				);

				convertedDescriptions.Add(convertedDescription);
			}

			if (convertedDescriptions.Count > 0)
				base.RaiseBattlePassDescriptionsConverted(convertedDescriptions);
			else
				Debug.LogWarning("No BattlePass descriptions converted");
		}

		private bool ConvertDateTime(string dateTime, out DateTime result)
		{
			if (DateTime.TryParseExact(dateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
				return true;
			else
			{
				Debug.LogError($"Could not parse DateTime: '{dateTime}'");
				return false;
			}
		}

		private bool DefineExpiration(DateTime expiryDateTime)
		{
			var currentDateTime = DateTime.Now;
			return CheckIfSoonerOrEqual(expiryDateTime, currentDateTime);
		}

		private bool CheckIfSoonerOrEqual(DateTime expiryDateTime, DateTime currentDateTime)
		{
			var difference = expiryDateTime - currentDateTime;
			var differenceInDays = difference.TotalDays;

			return differenceInDays <= 0.0;
		}

		private BattlePassLevelDescription[] ConvertLevels(BattlePassLevelDescriptionRaw[] originalLevels)
		{
			var result = new BattlePassLevelDescription[originalLevels.Length];

			for (int i = 0; i < originalLevels.Length; i++)
			{
				var originalLevel = originalLevels[i];
				var isLastLevel = i == originalLevels.Length - 1;

				var convertedFreeItem = originalLevel.FreeItem != null ? ConvertItem(originalLevel.FreeItem, originalLevel.Tier, isPremium: false, isFinal: false) : null;
				var convertedPremiumItem = originalLevel.PremiumItem != null ? ConvertItem(originalLevel.PremiumItem, originalLevel.Tier, isPremium: true, isFinal: isLastLevel) : null;

				var convertedLevel = new BattlePassLevelDescription
					(
						tier: originalLevel.Tier,
						experience: originalLevel.Experience,
						freeItem: convertedFreeItem,
						premiumItem: convertedPremiumItem
					);

				result[i] = convertedLevel;
			}

			return result;
		}

		private BattlePassItemDescription ConvertItem(BattlePassItemDescriptionRaw originalItem, int tier, bool isPremium, bool isFinal)
		{
			return new BattlePassItemDescription
				(
					sku: originalItem.Sku,
					promocode: originalItem.Promocode,
					quantity: originalItem.Quantity,
					tier: tier,
					isPremium: isPremium,
					isFinal:  isFinal
				);
		}
	}
}
