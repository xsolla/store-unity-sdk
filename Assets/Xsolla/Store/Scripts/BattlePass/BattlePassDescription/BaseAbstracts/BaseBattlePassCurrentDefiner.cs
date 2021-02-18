using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassCurrentDefiner : MonoBehaviour
	{
		public event Action<BattlePassDescription> CurrentBattlePassDefined;
		public abstract void OnBattlePassDescriptionsConverted(IEnumerable<BattlePassDescription> battlePassDescriptions);

		protected void RaiseCurrentBattlePassDefined(BattlePassDescription currentBattlePass) => CurrentBattlePassDefined?.Invoke(currentBattlePass);
	}
}
