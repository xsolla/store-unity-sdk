using System;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassCatalogExtractor : BaseBattlePassCatalogExtractor
	{
		private const string BATTLEPASS_GROUP = "#BATTLEPASS#";
		private const string BATTLEPASS_UTIL_SUFFIX = "util";

		public override void ExtractBattlePassItems()
		{
			var items = UserCatalog.Instance.AllItems;
			var inventoryDemoImplementation = DemoController.Instance.InventoryDemo;
			var battlepassItems = items.Where(item => inventoryDemoImplementation.GetCatalogGroupsByItem(item).Contains(BATTLEPASS_GROUP));
			battlepassItems = battlepassItems.Where(item => !item.Name.EndsWith(BATTLEPASS_UTIL_SUFFIX, StringComparison.InvariantCultureIgnoreCase));

			if (battlepassItems.Any())
				base.RaiseBattlePassItemsExtracted(battlepassItems);
			else
				Debug.LogWarning("No BattlePass items found");
		}
	}
}
