using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserPremiumStatusProvider : BaseBattlePassDescriptionSubscriber
    {
		[SerializeField] private InventoryFinder InventoryFinder = default;

		public event Action<bool> UserPremiumDefined;

		private string _battlePassName;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_battlePassName = battlePassDescription.Name;
			CheckUserPremium(onPurchase: false);
		}

		public void CheckUserPremium(bool onPurchase)
		{
			var onItemFound = new Action<InventoryItemModel>(_ => UserPremiumDefined?.Invoke(true));
			var onItemAbsence = new Action(() => UserPremiumDefined?.Invoke(false));

			var maxAttempts = onPurchase ? BattlePassConstants.MAX_INVENTORY_REFRESH_ATTEMPTS : 1;

			InventoryFinder.FindInInventory(_battlePassName, maxAttempts, onItemFound, onItemAbsence);
		}
	}
}
