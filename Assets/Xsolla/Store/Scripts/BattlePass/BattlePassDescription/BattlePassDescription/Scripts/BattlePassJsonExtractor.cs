using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Xsolla.Demo
{
	public class BattlePassJsonExtractor : BaseBattlePassJsonExtractor
	{
		public override void OnBattlePassItemsExtracted(IEnumerable<CatalogItemModel> battlePassItems)
		{
			var battlePassDescriptions = new List<BattlePassDescriptionRaw>();

			foreach (var item in battlePassItems)
			{
				var serializedDescription = item.LongDescription;

				if(!string.IsNullOrEmpty(serializedDescription))
				{
					var deserializedDescription = JsonConvert.DeserializeObject<BattlePassDescriptionRaw>(serializedDescription);
					battlePassDescriptions.Add(deserializedDescription);
				}
				else
					Debug.LogError(string.Format("BattlePass description is null or empty for item name: '{0}'", item.Name));
			}

			if (battlePassDescriptions.Count > 0)
				base.RaiseBattlePassJsonExtracted(battlePassDescriptions);
			else
				Debug.LogWarning("No BattlePass descriptions extracted");
		}
	}
}
