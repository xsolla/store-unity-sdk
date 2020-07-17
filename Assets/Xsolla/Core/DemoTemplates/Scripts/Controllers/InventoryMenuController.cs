using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private ItemContainer itemsContainer;
	[SerializeField] private GroupsController groupsController;
	
	private IDemoImplementation _demoImplementation;

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected virtual void Start()
	{
		_demoImplementation = DemoController.Instance.GetImplementation();
		groupsController.GroupSelectedEvent += PutItemsToContainer;
		StartCoroutine(InventoryCoroutine());
	}

	private void PutItemsToContainer(string groupName)
	{
		var items = new List<ItemModel>();
		UserInventory.Instance.AllItems.ForEach(i =>
		{
			if(i.IsVirtualCurrency()) return;
			if (i.IsSubscription())
			{
				if (!UserCatalog.Instance.Subscriptions.Any(sub => sub.Sku.Equals(i.Sku)))
				{
					Debug.LogError($"User subscription with sku = '{i.Sku}' have not equal catalog item!");
					return;
				}
			}
			else
			{
				if (!UserCatalog.Instance.VirtualItems.Any(cat => cat.Sku.Equals(i.Sku)))
				{
					Debug.LogError($"Inventory item with sku = '{i.Sku}' have not equal catalog item!");
					return;
				}
			}
			var catalogItem = UserCatalog.Instance.AllItems.First(cat => cat.Sku.Equals(i.Sku));
			if (_demoImplementation.GetCatalogGroupsByItem(catalogItem).Any(g => g.Equals(groupName)))
				items.Add(i);
		});
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
		var groups = new List<string>();
		if (items.Any())
			items.ForEach(i => groups.AddRange(_demoImplementation.GetCatalogGroupsByItem(i)));
		else
			groupsController.AddGroup("ALL");
		groups = groups.Distinct().ToList();
		groups.ForEach(groupName => groupsController.AddGroup(groupName));
		groupsController.SelectDefault();
	}
}
