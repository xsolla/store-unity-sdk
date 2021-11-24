using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassDescriptionConverter : MonoBehaviour
	{
		public event Action<IEnumerable<BattlePassDescription>> BattlePassDescriptionsConverted;
		public abstract void OnBattlePassJsonExtracted(IEnumerable<BattlePassDescriptionRaw> battleJsonItems);

		protected void RaiseBattlePassDescriptionsConverted(IEnumerable<BattlePassDescription> itemsConverted)
		{
			if (BattlePassDescriptionsConverted != null)
				BattlePassDescriptionsConverted.Invoke(itemsConverted);
		}
	}
}
