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

				var convertedDescription = new BattlePassDescription(
					name: item.Name,
					expiryDate: expiryDate,
					isExpired: isExpired,
					levels: item.Levels);

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
	}
}
