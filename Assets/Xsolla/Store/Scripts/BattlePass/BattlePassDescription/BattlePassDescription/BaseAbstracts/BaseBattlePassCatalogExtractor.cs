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
				var itemGroups = SdkCatalogLogic.Instance.GetCatalogGroupsByItem(item);
				return itemGroups.Count == 1 && itemGroups[0] == BattlePassConstants.BATTLEPASS_GROUP;
			});
			var targetItems = battlepassItems.Where(ItemPredicate);

			if (targetItems.Any())
			{
				if (BattlePassItemsExtracted != null)
					BattlePassItemsExtracted.Invoke(targetItems);
			}
			else
			{
				Debug.LogWarning(string.Format("No BattlePass items found that match: '{0}'", WarningNoItemsFoundIdentifier));
			}
		}
	}
}
