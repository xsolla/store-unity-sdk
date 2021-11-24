using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserPremiumStatusProvider : BaseBattlePassDescriptionSubscriber
    {
		[SerializeField] private InventoryFinder InventoryFinder;

		public event Action<bool> UserPremiumDefined;

		private string _battlePassName;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_battlePassName = battlePassDescription.Name;
			CheckUserPremium(onPurchase: false);
		}

		public void CheckUserPremium(bool onPurchase)
		{
			var onItemFound = new Action<InventoryItemModel>(_ =>
			{
				if (UserPremiumDefined != null)
					UserPremiumDefined.Invoke(true);
			});
			var onItemAbsence = new Action(() =>
			{
				if (UserPremiumDefined != null)
					UserPremiumDefined.Invoke(false);
			});

			var maxAttempts = onPurchase ? BattlePassConstants.MAX_INVENTORY_REFRESH_ATTEMPTS : 1;

			InventoryFinder.FindInInventory(_battlePassName, maxAttempts, onItemFound, onItemAbsence);
		}
	}
}
