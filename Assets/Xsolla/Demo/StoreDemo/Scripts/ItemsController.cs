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
	GameObject attributesContainerPrefab;

	[SerializeField]
	Transform content;

	private readonly Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();
	private GameObject _activeContainer;
	private bool _isEmptyCatalog;
	IExtraPanelController _extraController;

	void Awake()
	{
		_extraController = FindObjectOfType<ExtraController>();
		
		UserCart.Instance.ClearCartEvent += RefreshActiveContainer;
		UserInventory.Instance.UpdateItemsEvent += _ => RefreshActiveContainer();
	}

	public void CreateItems(List<StoreItem> items)
	{
		Dictionary<string, GameObject> defaultContainers = GetDefaultContainers();
		defaultContainers.ToList().ForEach(container => {
			AddContainer(container.Value, container.Key);
		});
		_isEmptyCatalog = !items.Any();
		if (!_isEmptyCatalog) {
			items.ForEach(i =>
			{
				if (i.groups.Any()) {
					i.groups.ToList().ForEach((group) => AddItemToContainer(group.name, i));
				} else {
					AddItemToContainer(Constants.UngroupedGroupName, i);
				}
			});
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
			{ Constants.InventoryContainerName, inventoryContainerPrefab },
			{ Constants.AttributesContainerName, attributesContainerPrefab }
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

	public List<ItemContainer> GetCatalogContainers()
	{
		Dictionary<string, GameObject> defaultContainers = GetDefaultContainers();
		List<KeyValuePair<string, GameObject>> catalog = _containers.Where(c => !defaultContainers.ContainsKey(c.Key)).ToList();
		if (_containers.ContainsKey(Constants.UngroupedGroupName)) {
			catalog.Add(new KeyValuePair<string, GameObject>(Constants.UngroupedGroupName, _containers[Constants.UngroupedGroupName]));
		}		
		return catalog.Select(k => k.Value.GetComponent<ItemContainer>()).ToList();
	}

	public void AddVirtualCurrencyPackages(List<VirtualCurrencyPackage> packages)
	{
		var groupContainer = _containers[Constants.CurrencyGroupName];
		packages.ForEach(groupContainer.GetComponent<ItemContainer>().AddItem);
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
		_activeContainer = InternalActivateContainer(_containers.ContainsKey(groupId)
			? groupId
			: Constants.EmptyContainerName
		);
		_activeContainer.GetComponent<IContainer>().Refresh();
		ItemContainer itemContainer = _activeContainer.GetComponent<ItemContainer>();

		if (_isEmptyCatalog && (itemContainer != null) && (itemContainer.Items.Count == 0)) {
			
			itemContainer.EnableEmptyContainerMessage();
		}

		_extraController.SetAttributesVisibility(groupId != Constants.AttributesContainerName);
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