using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class InventoryItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject itemPrefab;

	[SerializeField]
	Transform itemParent;

	List<GameObject> _items;
	
	StoreController _storeController;

	void Awake()
	{
		_items = new List<GameObject>();
		
		_storeController = FindObjectOfType<StoreController>();
	}

	public void AddItem(InventoryItem itemInformation)
	{
		var newItem = Instantiate(itemPrefab, itemParent);
		newItem.GetComponent<InventoryItemUI>().Initialize(itemInformation);
		_items.Add(newItem);
	}

	public void Refresh()
	{
		ClearInventoryItems();

		foreach (var item in _storeController.inventory.items)
		{
			AddItem(item);
		}
	}
	
	void ClearInventoryItems()
	{
		foreach (var item in _items)
		{
			Destroy(item);
		}
		
		_items.Clear();
	}
}