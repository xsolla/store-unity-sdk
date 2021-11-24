using System;
using System.Collections.Generic;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public partial class AttributesUserStatManager : BaseBattlePassUserStatManager
	{
		private BattlePassUserStat GenerateBattlePassUserStat(UserAttribute levelAttribute, UserAttribute expAttribute, UserAttribute obtainedAttribute)
		{
			int level; int exp;
			GetLevelAndExp(levelAttribute, expAttribute, out level, out exp);

			int[] obtainedFreeItems; int[] obtainedPremiumItems;
			GetObtainedItems(obtainedAttribute, out obtainedFreeItems, out obtainedPremiumItems);

			return new BattlePassUserStat(level, exp, obtainedFreeItems, obtainedPremiumItems);
		}

		private void GetLevelAndExp(UserAttribute levelAttribute, UserAttribute expAttribute, out int level, out int exp)
		{
			var originalLevel = int.Parse(levelAttribute.value);
			var originalExp = int.Parse(expAttribute.value);

			int recalculatedLevel; int recalculatedExp;
			RecalculateLevelAndExp(_battlePassDescription.Levels, originalLevel, originalExp, out recalculatedLevel, out recalculatedExp);

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
				Debug.LogError(
					string.Format("Something went wrong. Level index based on user level was: '{0}'. Levels length: '{1}'. User level: '{2}'", levelIndex, battlePassLevels.Length, originalLevel));

				if (levelIndex < 0)
				{
					Debug.Log("Setting user level to one.");
					newLevel = 1;
				}
				else
				{
					Debug.Log("Setting user level to max.");
					newLevel = battlePassLevels.Length - 1;
				}
				return;
			}

			while (newExp >= battlePassLevels[levelIndex].Experience)
			{
				if (levelIndex < battlePassLevels.Length - 1)
				{
					newLevel++;
					newExp -= battlePassLevels[levelIndex].Experience;
					levelIndex++;
				}
				else
				{
					newExp = battlePassLevels[levelIndex-1].Experience;
					break;
				}
			}
		}

		private void GetObtainedItems(UserAttribute obtainedAttribute, out int[] obtainedFreeItems, out int[] obtainedPremiumItems)
		{
			try
			{
				var valueParts = obtainedAttribute.value.Split(' ');
				var freeItemsPart = valueParts[0];
				var premiumItemsPart = valueParts[1];

				obtainedFreeItems = GetNumbersFromPart(freeItemsPart);
				obtainedPremiumItems = GetNumbersFromPart(premiumItemsPart);
			}
			catch (Exception ex)
			{
				obtainedFreeItems = new int[0];
				obtainedPremiumItems = new int[0];

				Debug.LogError(string.Format("Error parsing obtained items record. Record:'{0}'{1}Error:'{2}'", obtainedAttribute.value, Environment.NewLine, ex.Message));
			}
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
