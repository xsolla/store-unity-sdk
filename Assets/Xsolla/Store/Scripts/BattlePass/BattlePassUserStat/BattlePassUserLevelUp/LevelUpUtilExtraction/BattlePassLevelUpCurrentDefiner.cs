using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpCurrentDefiner : MonoBehaviour
    {
		public event Action<CatalogItemModel> CurrentLevelUpDefined;

		public void DefineCurrent(IEnumerable<CatalogItemModel> levelUpUtils, string battlePassName)
		{
			var result = new List<CatalogItemModel>();

			foreach (var item in levelUpUtils)
				if (item.Name.Contains(battlePassName))
					result.Add(item);

			if (result.Count > 0)
			{
				if (result.Count > 1)
					Debug.LogWarning($"More than one level up util matches current battle pass name: '{battlePassName}'. Taking first.");

				CurrentLevelUpDefined?.Invoke(result[0]);
			}
			else
				Debug.LogWarning($"No level up util matches current battle pass name: '{battlePassName}'.");
		}
	}
}
