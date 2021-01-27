using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseStorePageItemsController : BasePageItemsController
	{
		protected IStoreDemoImplementation _storeDemoImplementation;

		protected override void Initialize()
		{
			_storeDemoImplementation = DemoController.Instance.StoreDemo;
		}

		protected override void InitializeItemUI(GameObject item, ItemModel model)
		{
			item.GetComponent<ItemUI>().Initialize((CatalogItemModel)model, _storeDemoImplementation);
		}
	}
}
