using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class InventoryItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject itemPrefab;

	[SerializeField]
	Transform itemParent;

	private List<GameObject> _items;

	private void Awake()
	{
		_items = new List<GameObject>();
	}

	public void AddItem(InventoryItem itemInformation)
	{
		var newItem = Instantiate(itemPrefab, itemParent);
		newItem.GetComponent<InventoryItemUI>().Initialize(itemInformation);
		_items.Add(newItem);
	}

	private void ClearItems()
	{
		_items.ForEach(Destroy);
		_items.Clear();
	}
	
	public void Refresh()
	{
		ClearItems();
		UserInventory.Instance.GetItems().ForEach(AddItem);
	}
}