using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Demo
{
	public class StorePageItemsController : BaseStorePageItemsController
	{
		private IInventoryDemoImplementation _inventoryDemoImplementation;

		protected override void Initialize()
		{
			base.Initialize();
			_inventoryDemoImplementation = DemoController.Instance.InventoryDemo;
		}

		protected override List<ItemModel> GetItemsByGroup(string groupName)
		{
			var items = groupName.Equals(GROUP_ALL)
				? UserCatalog.Instance.AllItems.Where(i => !i.IsVirtualCurrency()).ToList()
				: UserCatalog.Instance.AllItems.Where(i => 
					_inventoryDemoImplementation.GetCatalogGroupsByItem(i).Contains(groupName)).ToList();

			return items.Cast<ItemModel>().ToList();
		}

		protected override List<string> GetGroups()
		{
			var items = UserCatalog.Instance.AllItems;
			var groups = new List<string>();

			items.ForEach(i => groups.AddRange(_inventoryDemoImplementation.GetCatalogGroupsByItem(i)));
			groups = groups.Distinct().ToList();
			groups.Remove(GROUP_ALL);

			return groups;
		}
	}
}
