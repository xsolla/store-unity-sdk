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

	public void CreateItems(StoreItems items)
	{
		AddContainer(itemsContainerPrefab, Constants.EmptyContainerName);
		AddContainer(itemsContainerPrefab, Constants.CurrencyGroupName);
		AddContainer(cartContainerPrefab, Constants.CartGroupName);
		AddContainer(inventoryContainerPrefab, Constants.InventoryContainerName);

		List<StoreItem> list = items.items.ToList();
		if(list.Any()) {
			foreach (var item in list) {
				if (item.groups.Any()) {
					foreach (var group in item.groups) {
						if (!_containers.ContainsKey(group.name)) {
							AddContainer(itemsContainerPrefab, group.name);
						}

						var groupContainer = _containers[group.name];
						groupContainer.GetComponent<ItemContainer>().AddItem(item);
					}
				} else {
					if (!_containers.ContainsKey(Constants.UngroupedGroupName)) {
						AddContainer(itemsContainerPrefab, Constants.UngroupedGroupName);
					}

					var groupContainer = _containers[Constants.UngroupedGroupName];
					groupContainer.GetComponent<ItemContainer>().AddItem(item);
				}
			}
		} else {
			ActivateContainer(Constants.EmptyContainerName);
			_containers[Constants.EmptyContainerName].GetComponent<ItemContainer>().EnableEmptyContainerMessage();
		}
	}

	public void AddVirtualCurrencyPackage(VirtualCurrencyPackages items)
	{
		var groupContainer = _containers[Constants.CurrencyGroupName];
		foreach (var item in items.items) {
			groupContainer.GetComponent<ItemContainer>().AddItem(item);
		}
	}

	void AddContainer(GameObject itemContainerPref, string containerName)
	{
		var newContainer = Instantiate(itemContainerPref, content);
		newContainer.name = containerName;
		newContainer.SetActive(false);
		_containers.Add(containerName, newContainer);
	}

	public void ActivateContainer(string groupId)
	{
		foreach (var container in _containers.Values)
		{
			container.SetActive(false);
		}

		if (_containers.ContainsKey(groupId))
		{
			_containers[groupId].SetActive(true);
			_containers[groupId].GetComponent<IContainer>().Refresh();
		} else {
			ActivateContainer(Constants.EmptyContainerName);
			_containers[Constants.EmptyContainerName].GetComponent<ItemContainer>().EnableEmptyContainerMessage();
		}
	}

	public void RefreshActiveContainer()
	{
		IEnumerable<KeyValuePair<string, GameObject>> activeContainers = _containers.Where(c => c.Value.activeSelf);
		if(activeContainers.Count() > 0) {
			activeContainers.First().Value.GetComponent<IContainer>().Refresh();
		}
	}
}