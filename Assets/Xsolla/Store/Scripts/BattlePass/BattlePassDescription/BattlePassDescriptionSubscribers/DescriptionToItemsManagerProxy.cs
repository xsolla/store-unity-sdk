using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToItemsManagerProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassItemsManager ItemsManager;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			ItemsManager.OnBattlePassDescriptionArrived(battlePassDescription);
		}
	}
}
