using System.Collections;
using System.Collections.Generic;
using System;
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

		var predicate = groupName.Equals(ALL_ITEMS_GROUP) ? new Func<string, bool>(_ => true) : g => g.Equals(groupName);

		var items = UserInventory.Instance.AllItems.Where(i =>
			{
				if(i.IsVirtualCurrency()) return false;
				if (i.IsSubscription())
				{
					if (!UserCatalog.Instance.Subscriptions.Any(sub => sub.Sku.Equals(i.Sku)))
					{
						Debug.LogError($"User subscription with sku = '{i.Sku}' have not equal catalog item!");
						return false;
					}

					var model = UserInventory.Instance.Subscriptions.First(x => x.Sku.Equals(i.Sku));
					if (!(model.Status != UserSubscriptionModel.SubscriptionStatusType.None && model.Expired.HasValue))
					{
						return false;//This is a non-purchased subscription
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
				return _demoImplementation.GetCatalogGroupsByItem(catalogItem).Any(predicate);
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
		CreateAndFillInventoryGroups(UserInventory.Instance.AllItems);
	}

	private void CreateAndFillInventoryGroups(List<ItemModel> items)
	{
		groupsController.AddGroup(ALL_ITEMS_GROUP);

		var itemGroups = new HashSet<string>();

		foreach (var item in items)
		{
			if (item.IsSubscription() && item is UserSubscriptionModel subscription && subscription.Status == UserSubscriptionModel.SubscriptionStatusType.None)
			{
				//Do nothing, skip this item
				continue;
			}
			else
			{
				var currentItemGroups = _demoImplementation.GetCatalogGroupsByItem(item);

				foreach (var group in currentItemGroups)
					itemGroups.Add(group);
			}
		}

		foreach (var group in itemGroups)
			groupsController.AddGroup(group);

		groupsController.SelectDefault();
	}
}
