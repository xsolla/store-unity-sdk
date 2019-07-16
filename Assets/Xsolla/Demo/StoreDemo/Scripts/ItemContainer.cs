using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject itemPrefab;

	[SerializeField]
	Transform itemParent;

	List<ItemUI> _items;

	void Awake()
	{
		_items = new List<ItemUI>();
	}

	public void AddItem(Xsolla.Store.StoreItem itemInformation)
	{
		var item = Instantiate(itemPrefab, itemParent).GetComponent<ItemUI>();
		item.Initialize(itemInformation);
		
		_items.Add(item);
	}

	public void Refresh()
	{
		foreach (var item in _items)
		{
			item.Refresh();
		}
	}
}