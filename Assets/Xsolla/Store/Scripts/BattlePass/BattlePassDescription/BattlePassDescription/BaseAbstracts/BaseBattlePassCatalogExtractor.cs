using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassCatalogExtractor : MonoBehaviour
	{
		protected abstract Func<CatalogItemModel, bool> ItemPredicate { get; }
		protected abstract string WarningNoItemsFoundIdentifier { get; }

		public event Action<IEnumerable<CatalogItemModel>> BattlePassItemsExtracted;

		public void ExtractBattlePassItems()
		{
			var allItems = UserCatalog.Instance.AllItems;
			var battlepassItems = allItems.Where(item =>
			{
				var itemGroups = item.Groups;
				return itemGroups.Count == 1 && itemGroups[0] == BattlePassConstants.BATTLEPASS_GROUP;
			});
			var targetItems = battlepassItems.Where(ItemPredicate);

			if (targetItems.Any())
				BattlePassItemsExtracted?.Invoke(targetItems);
			else
				Debug.LogWarning($"No BattlePass items found that match: '{WarningNoItemsFoundIdentifier}'");
		}
	}
}
