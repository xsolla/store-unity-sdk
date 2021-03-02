using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public partial class AttributesUserStatManager : BaseBattlePassUserStatManager
	{
		private BattlePassUserStat GenerateBattlePassUserStat(UserAttribute levelAttribute, UserAttribute expAttribute, UserAttribute obtainedAttribute)
		{
			GetLevelAndExp(levelAttribute, expAttribute, out int level, out int exp);
			GetObtainedItems(obtainedAttribute, out int[] obtainedFreeItems, out int[] obtainedPremiumItems);

			return new BattlePassUserStat(level, exp, obtainedFreeItems, obtainedPremiumItems);
		}

		private void GetLevelAndExp(UserAttribute levelAttribute, UserAttribute expAttribute, out int level, out int exp)
		{
			var originalLevel = int.Parse(levelAttribute.value);
			var originalExp = int.Parse(expAttribute.value);

			RecalculateLevelAndExp(_battlePassDescription.Levels, originalLevel, originalExp, out var recalculatedLevel, out var recalculatedExp);

			if (originalLevel == recalculatedLevel)
				level = originalLevel;
			else
			{
				level = recalculatedLevel;
				levelAttribute.value = recalculatedLevel.ToString();
				_attributesToUpdate.Add(levelAttribute);
			}

			if (originalExp == recalculatedExp)
				exp = originalExp;
			else
			{
				exp = recalculatedExp;
				expAttribute.value = recalculatedExp.ToString();
				_attributesToUpdate.Add(expAttribute);
			}
		}

		private void RecalculateLevelAndExp(BattlePassLevelDescription[] battlePassLevels, int originalLevel, int originalExp, out int newLevel, out int newExp)
		{
			newLevel = originalLevel;
			newExp = originalExp;

			var levelIndex = originalLevel - 1;

			if (levelIndex < 0 || levelIndex >= battlePassLevels.Length)
			{
				Debug.LogError($"Something went wrong. Level index based on user level was: '{levelIndex}'. Levels length: '{battlePassLevels.Length}'. User level: '{originalLevel}'");
				Debug.Log("Setting user level to one.");
				newLevel = 1;
				return;
			}

			while (newExp >= battlePassLevels[levelIndex].Experience)
			{
				newLevel++;
				newExp -= battlePassLevels[levelIndex].Experience;
				
				if (levelIndex != battlePassLevels.Length - 1)
					levelIndex++;
				else
				{
					newExp = battlePassLevels[levelIndex].Experience;
					break;
				}
			}
		}

		private void GetObtainedItems(UserAttribute obtainedAttribute, out int[] obtainedFreeItems, out int[] obtainedPremiumItems)
		{
			var valueParts = obtainedAttribute.value.Split(' ');
			var freeItemsPart = valueParts[0];
			var premiumItemsPart = valueParts[1];

			obtainedFreeItems = GetNumbersFromPart(freeItemsPart);
			obtainedPremiumItems = GetNumbersFromPart(premiumItemsPart);
		}

		private int[] GetNumbersFromPart(string itemsPart)
		{
			var subparts = itemsPart.Split(OBTAINED_VALUE_SEPARATOR);
			var result = new List<int>(subparts.Length-1);

			for (int i = 1; i < subparts.Length; i++)
			{
				if (!string.IsNullOrEmpty(subparts[i]))
				{
					var number = int.Parse(subparts[i]);
					result.Add(number);
				}
			}

			return result.ToArray();
		}
	}
}
