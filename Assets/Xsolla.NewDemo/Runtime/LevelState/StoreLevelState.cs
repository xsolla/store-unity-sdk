using System.Collections;

namespace Xsolla.Demo
{
	public class StoreLevelState : ILevelState, ILevelStateExit
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();
		private IStoreService StoreService => ServiceLocator.Resolve<IStoreService>();
		private IInventoryService InventoryService => ServiceLocator.Resolve<IInventoryService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		public void OnEnter()
		{
			var levelScreen = ScreenService.OpenLevelPlayScreen();
			ScreenService.Close(levelScreen);

			TimeService.Freeze();
			ScreenService.OpenStoreScreen();

			StoreService.OnPurchaseSuccess += OnPurchaseSuccess;
		}

		public void OnExit()
		{
			StoreService.OnPurchaseSuccess -= OnPurchaseSuccess;
		}

		private void OnPurchaseSuccess(PurchaseInfo purchaseInfo)
		{
			CoroutineExecutor.Run(HandlePurchaseSuccess(purchaseInfo));
		}

		private IEnumerator HandlePurchaseSuccess(PurchaseInfo purchaseInfo)
		{
			yield return InventoryService.FetchInventoryCoroutine();

			if (IsOwlPurchasedFirstTime(purchaseInfo))
				LevelStateMachine.SetOwlPurchased();
			else
				LevelStateMachine.SetResumeLevel();
		}

		private bool IsOwlPurchasedFirstTime(PurchaseInfo purchaseInfo)
		{
			var owlSku = "owl";

			if (purchaseInfo.Sku != owlSku)
				return false;

			return InventoryService.GetItem(owlSku).Quantity == 1;
		}
	}
}