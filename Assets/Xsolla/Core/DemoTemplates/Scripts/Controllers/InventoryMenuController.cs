using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
	private const string ALL_ITEMS_GROUP = "ALL";
	
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private ItemContainer itemsContainer;
	[SerializeField] private GroupsController groupsController;
	
	private IDemoImplementation _demoImplementation;
	private string _group;

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected virtual void Start()
	{
		_demoImplementation = DemoController.Instance.GetImplementation();
		groupsController.GroupSelectedEvent += PutItemsToContainer;
		StartCoroutine(InventoryCoroutine());
		UserInventory.Instance.RefreshEvent += () => PutItemsToContainer(_group);
	}

	private void PutItemsToContainer(string groupName)
	{
		_group = groupName;
		var items = (groupName.Equals(ALL_ITEMS_GROUP))
			? UserInventory.Instance.AllItems
			: UserInventory.Instance.AllItems.Where(i =>
			{
				if(i.IsVirtualCurrency()) return false;
				if (i.IsSubscription())
				{
					if (!UserCatalog.Instance.Subscriptions.Any(sub => sub.Sku.Equals(i.Sku)))
					{
						Debug.LogError($"User subscription with sku = '{i.Sku}' have not equal catalog item!");
						return false;
					}
				}
				else
				{
					if (!UserCatalog.Instance.VirtualItems.Any(cat => cat.Sku.Equals(i.Sku)))
					{
						Debug.LogError($"Inventory item with sku = '{i.Sku}' have not equal catalog item!");
						return false;
					}
				}
				var catalogItem = UserCatalog.Instance.AllItems.First(cat => cat.Sku.Equals(i.Sku));
				return _demoImplementation.GetCatalogGroupsByItem(catalogItem).Any(g => g.Equals(groupName));
			}).ToList();
		
		itemsContainer.Clear();
		items.ForEach(i =>
		{
			var go = itemsContainer.AddItem(itemPrefab);
			go.GetComponent<InventoryItemUI>().Initialize(i, _demoImplementation);
		}); 
	}

	private IEnumerator InventoryCoroutine()
	{
		yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);
		CreateAndFillInventoryGroups(UserCatalog.Instance.AllItems);
	}

	private void CreateAndFillInventoryGroups(List<CatalogItemModel> items)
	{
		var groups = new List<string>{ALL_ITEMS_GROUP};
		groupsController.AddGroup(ALL_ITEMS_GROUP);
		
		if (items.Any())
			items.ForEach(i => groups.AddRange(_demoImplementation.GetCatalogGroupsByItem(i)));
		groups = groups.Distinct().ToList();
		groups.Remove(ALL_ITEMS_GROUP);
		groups.ForEach(groupName => groupsController.AddGroup(groupName));
		groupsController.SelectDefault();
	}
}
