using UnityEngine;

namespace Xsolla.Demo
{
	public class StoreScreen : Screen
	{
		private const string HEART_ITEM_SKU = "heart";
		private const string OWL_ITEM_SKU = "owl";
		private const string COIN_PACK_ITEM_SKU = "coin_pack";

		[field: SerializeField] private WidgetEventsProvider BackgroundEventsProvider { get; set; }
		[field: SerializeField] private StoreItemWidget HeartWidget { get; set; }
		[field: SerializeField] private StoreItemWidget OwlWidget { get; set; }
		[field: SerializeField] private StoreItemWidget CoinPackWidget { get; set; }

		private ICatalogService CatalogService => ServiceLocator.Resolve<ICatalogService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		private void Start()
		{
			BackgroundEventsProvider.PointerDown += _ => LevelStateMachine.SetResumeLevel();

			var item = CatalogService.GetItem(HEART_ITEM_SKU);
			if (item != null)
				HeartWidget.Setup(item);
			else
				Debug.LogError($"Heart item not found in catalog: {HEART_ITEM_SKU}");

			item = CatalogService.GetItem(OWL_ITEM_SKU);
			if (item != null)
				OwlWidget.Setup(item);
			else
				Debug.LogError($"Owl item not found in catalog: {OWL_ITEM_SKU}");

			item = CatalogService.GetItem(COIN_PACK_ITEM_SKU);
			if (item != null)
				CoinPackWidget.Setup(item);
			else
				Debug.LogError($"Coin pack item not found in catalog: {COIN_PACK_ITEM_SKU}");
		}
	}
}