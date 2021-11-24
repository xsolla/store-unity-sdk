using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public partial class AttributesUserStatManager : BaseBattlePassUserStatManager
	{
		public override void AddLevel(int levelsToAdd)
		{
			var newUserLevel = _currentUserStat.Level + levelsToAdd;
			_levelAttribute.value = newUserLevel.ToString();
			UpdateAttribute(_levelAttribute);
			IssueNewUserStat();
		}

		public override void AddExp(int expToAdd)
		{
			var newUserExp = _currentUserStat.Exp + expToAdd;
			_expAttribute.value = newUserExp.ToString();
			UpdateAttribute(_expAttribute);
			IssueNewUserStat();
		}

		public override void AddObtainedItems(int[] freeItemsToAdd = null, int[] premiumItemsToAdd = null)
		{
			if (freeItemsToAdd == null && premiumItemsToAdd == null)
			{
				Debug.LogWarning("AddObtainedItems: Both arguments were null. At least one must be provided.");
				return;
			}

			var newObtainedFreeItems = freeItemsToAdd == null ? _currentUserStat.ObtainedFreeItems : JoinItems(_currentUserStat.ObtainedFreeItems, freeItemsToAdd);
			var newObtainedPremiumItems = premiumItemsToAdd == null ? _currentUserStat.ObtainedPremiumItems : JoinItems(_currentUserStat.ObtainedPremiumItems, premiumItemsToAdd);

			var newAttributeValue = GenerateObtainedItemsValue(newObtainedFreeItems, newObtainedPremiumItems);

			_obtainedAttribute.value = newAttributeValue;
			UpdateAttribute(_obtainedAttribute);
			IssueNewUserStat();
		}

		private int[] JoinItems(int[] originalItems, int[] itemsToAdd)
		{
			var allItems = new List<int>();
			allItems.AddRange(originalItems);
			allItems.AddRange(itemsToAdd);

			var result = new List<int>();

			foreach (var item in allItems)
				if (!result.Contains(item))
					result.Add(item);

			result.Sort();

			return result.ToArray();
		}

		private string GenerateObtainedItemsValue(int[] obtainedFreeItems, int[] obtainedPremiumItems)
		{
			var freeItemsSubValue = GenerateObtainedSubValue(obtainedFreeItems);
			var premiumItemsSubValue = GenerateObtainedSubValue(obtainedPremiumItems);
			
			return string.Format(OBTAINED_VALUE_TEMPLATE, freeItemsSubValue, premiumItemsSubValue);
		}

		private string GenerateObtainedSubValue(int[] obtainedItems)
		{
			var builder = new StringBuilder();

			foreach (var item in obtainedItems)
				builder.Append(OBTAINED_VALUE_SEPARATOR).Append(item);

			return builder.ToString();
		}

		private void UpdateAttribute(UserAttribute userAttribute)
		{
			_attributesToUpdate.Add(userAttribute);
			UpdateAttributesIfAny();
		}
	}
}
