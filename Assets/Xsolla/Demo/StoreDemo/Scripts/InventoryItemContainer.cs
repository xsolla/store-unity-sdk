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

	void Awake()
	{
		_items = new List<GameObject>();
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
		
		XsollaStore.Instance.GetInventoryItems((items =>
		{
			foreach (var item in items.items)
			{
				AddItem(item);
			}
		}), print);
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