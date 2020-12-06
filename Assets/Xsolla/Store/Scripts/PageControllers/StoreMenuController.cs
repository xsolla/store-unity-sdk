using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class StoreMenuController : MonoBehaviour
	{
		private const string ALL_ITEMS_GROUP = "ALL";
	
		[SerializeField] private GameObject itemPrefab = default;
		[SerializeField] private GroupsController groupsController = default;
		[SerializeField] private ItemContainer itemsContainer = default;
		private IInventoryDemoImplementation _inventoryDemoImplementation;
		private IStoreDemoImplementation _storeDemoImplementation;

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		protected virtual void Start()
		{
			_inventoryDemoImplementation = DemoController.Instance.InventoryDemo;
			_storeDemoImplementation = DemoController.Instance.StoreDemo;
			groupsController.GroupSelectedEvent += PutItemsToContainer;
			StartCoroutine(CatalogCoroutine());
		}

		private void PutItemsToContainer(string groupName)
		{
			var items = (groupName.Equals(ALL_ITEMS_GROUP))
				? UserCatalog.Instance.AllItems.Where(i => !i.IsVirtualCurrency()).ToList()
				: UserCatalog.Instance.AllItems.Where(i => 
					_inventoryDemoImplementation.GetCatalogGroupsByItem(i).Contains(groupName)).ToList();
		
			itemsContainer.Clear();
			items.ForEach(i =>
			{
				var go = itemsContainer.AddItem(itemPrefab);
				go.GetComponent<ItemUI>().Initialize(i, _storeDemoImplementation);
			}); 
		}

		private IEnumerator CatalogCoroutine()
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);
			CreateAndFillCatalogGroups(UserCatalog.Instance.AllItems);
		}

		private void CreateAndFillCatalogGroups(List<CatalogItemModel> items)
		{
			List<string> groups = new List<string> {ALL_ITEMS_GROUP};
			groupsController.AddGroup(ALL_ITEMS_GROUP);
		
			if (items.Any())
				items.ForEach(i => groups.AddRange(_inventoryDemoImplementation.GetCatalogGroupsByItem(i)));
			groups = groups.Distinct().ToList();
			groups.Remove(ALL_ITEMS_GROUP);
			groups.ForEach(groupName => groupsController.AddGroup(groupName));
			groupsController.SelectDefault();
		}
	}
}
