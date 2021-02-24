using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassCatalogExtractor : MonoBehaviour
	{
		public event Action<IEnumerable<CatalogItemModel>> BattlePassItemsExtracted;
		public abstract void ExtractBattlePassItems();

		protected void RaiseBattlePassItemsExtracted(IEnumerable<CatalogItemModel> itemsExtracted) => BattlePassItemsExtracted?.Invoke(itemsExtracted);
	}
}
