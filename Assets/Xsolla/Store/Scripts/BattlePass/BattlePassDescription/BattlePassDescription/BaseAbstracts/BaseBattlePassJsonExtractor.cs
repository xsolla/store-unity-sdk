using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassJsonExtractor : MonoBehaviour
	{
		public event Action<IEnumerable<BattlePassDescriptionRaw>> BattlePassJsonExtracted;
		public abstract void OnBattlePassItemsExtracted(IEnumerable<CatalogItemModel> battlePassItems);

		protected void RaiseBattlePassJsonExtracted(IEnumerable<BattlePassDescriptionRaw> itemsExtracted)
		{
			if (BattlePassJsonExtracted != null)
				BattlePassJsonExtracted.Invoke(itemsExtracted);
		}
	}
}
