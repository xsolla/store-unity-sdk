using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryMenuController : MonoBehaviour
	{
		private const string ALL_ITEMS_GROUP = "ALL";
	
		[SerializeField] private GameObject itemPrefab = default;
		[SerializeField] private ItemContainer itemsContainer = default;
		[SerializeField] private GroupsController groupsController = default;

		private IInventoryDemoImplementation _demoImplementation;
		private string _group;

		protected virtual void Start()
		{
			_demoImplementation = DemoController.Instance.InventoryDemo;
			groupsController.GroupSelectedEvent += PutItemsToContainerOnGroupSelect;
			StartCoroutine(InventoryCoroutine());
			UserInventory.Instance.RefreshEvent += OnUserInventoryRefresh;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			UserInventory.Instance.RefreshEvent -= OnUserInventoryRefresh;
		}

		private void OnUserInventoryRefresh()
		{
			PutItemsToContainerOnRefreshEvent(_group);
		}

		private void PutItemsToContainerOnGroupSelect(string groupName)
		{
			PutItemsToContainer(groupName);
		}

		private void PutItemsToContainerOnRefreshEvent(string groupName)
		{
			if (string.IsNullOrEmpty(groupName))
				groupName = ALL_ITEMS_GROUP;

			PutItemsToContainer(groupName);
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
							Debug.Log($"User subscription with sku = '{i.Sku}' have not equal catalog item!");
							return true;
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
							Debug.Log($"Inventory item with sku = '{i.Sku}' have not equal catalog item!");
							return true;
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
}
