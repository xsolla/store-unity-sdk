using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public class ItemsController : MonoBehaviour
{
	[SerializeField]
	GameObject itemsContainerPrefab;

	[SerializeField]
	GameObject cartContainerPrefab;

	[SerializeField]
	GameObject inventoryContainerPrefab;

	[SerializeField]
	Transform content;
	
	Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();

	private GameObject activeContainer;
	private bool isEmptyCatalog;

	public void CreateItems(StoreItems items)
	{
		Dictionary<string, GameObject> defaultContainers = GetDefaultContainers();
		defaultContainers.ToList().ForEach((KeyValuePair<string, GameObject> container) => {
			AddContainer(container.Value, container.Key);
		});

		List<StoreItem> list = items.items.ToList();
		isEmptyCatalog = !list.Any();
		if (!isEmptyCatalog) {
			foreach (var item in list)  {
				if (item.groups.Any()) {
					item.groups.ToList().ForEach((group) => AddItemToContainer(group.name, item));
				} else {
					AddItemToContainer(Constants.UngroupedGroupName, item);
				}
			}
		} else {
			ActivateContainer(Constants.EmptyContainerName);
		}
	}

	private void AddItemToContainer(string containerName, StoreItem item)
	{
		GameObject container = _containers.ContainsKey(containerName)
			? _containers[containerName]
			: AddContainer(itemsContainerPrefab, containerName);
		container.GetComponent<ItemContainer>().AddItem(item);
	}

	private Dictionary<string, GameObject> GetDefaultContainers()
	{
		Dictionary<string, GameObject> itemContainers = GetDefaultItemContainers();
		Dictionary<string, GameObject> otherContainers = new Dictionary<string, GameObject>() {
			{ Constants.CartGroupName, cartContainerPrefab },
			{ Constants.InventoryContainerName, inventoryContainerPrefab }
		};
		itemContainers.ToList().ForEach((container) => { otherContainers.Add(container.Key, container.Value); });
		return otherContainers;
	}

	private Dictionary<string, GameObject> GetDefaultItemContainers()
	{
		return new Dictionary<string, GameObject>() {
			{ Constants.EmptyContainerName, itemsContainerPrefab },
			{ Constants.CurrencyGroupName, itemsContainerPrefab },
			{ Constants.UngroupedGroupName, itemsContainerPrefab }
		};
	}

	public void AddVirtualCurrencyPackage(VirtualCurrencyPackages items)
	{
		var groupContainer = _containers[Constants.CurrencyGroupName];
		foreach (var item in items.items) {
			groupContainer.GetComponent<ItemContainer>().AddItem(item);
		}
	}

	GameObject AddContainer(GameObject itemContainerPref, string containerName)
	{
		var newContainer = Instantiate(itemContainerPref, content);
		newContainer.name = containerName;
		newContainer.SetActive(false);
		_containers.Add(containerName, newContainer);
		return newContainer;
	}

	public void ActivateContainer(string groupId)
	{
		activeContainer = InternalActivateContainer(_containers.ContainsKey(groupId)
			? groupId
			: Constants.EmptyContainerName
		);
		activeContainer.GetComponent<IContainer>().Refresh();
		ItemContainer itemContainer = activeContainer.GetComponent<ItemContainer>();

		if (isEmptyCatalog && (itemContainer != null) && (itemContainer.Items.Count == 0)) {
			
				itemContainer.EnableEmptyContainerMessage();
		}
	}

	private GameObject InternalActivateContainer(string containerName)
	{
		foreach (var container in _containers.Values) {
			container.SetActive(false);
		}
		GameObject result = _containers[containerName];
		result.SetActive(true);
		return result;
	}

	public void RefreshActiveContainer()
	{
		IEnumerable<KeyValuePair<string, GameObject>> activeContainers = _containers.Where(c => c.Value.activeSelf);
		if(activeContainers.Count() > 0) {
			activeContainers.First().Value.GetComponent<IContainer>().Refresh();
		}
	}
}