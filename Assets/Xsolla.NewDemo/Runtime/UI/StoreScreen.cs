using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class StoreScreen : Screen
	{
		[field: SerializeField] private Button CloseButton { get; set; }
		[field: SerializeField] private StoreItemWidget HeartWidget { get; set; }
		[field: SerializeField] private StoreItemWidget OwlWidget { get; set; }
		[field: SerializeField] private StoreItemWidget CoinPackWidget { get; set; }

		private ICatalogService CatalogService => ServiceLocator.Resolve<ICatalogService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		private void Start()
		{
			var item = CatalogService.GetItem("heart");
			HeartWidget.Setup(item);

			item = CatalogService.GetItem("owl");
			OwlWidget.Setup(item);

			item = CatalogService.GetItem("coin_pack");
			CoinPackWidget.Setup(item);

			CloseButton.onClick.AddListener(() => LevelStateMachine.SetResumeLevel());
		}
	}
}