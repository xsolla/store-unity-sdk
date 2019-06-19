using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	[SerializeField]
	GameObject _item_Prefab;

	[SerializeField]
	Transform _item_Parent;

	List<ItemUI> _items;

	void Awake()
	{
		_items = new List<ItemUI>();
	}

	public void AddItem(Xsolla.StoreItem itemInformation)
	{
		var item = Instantiate(_item_Prefab, _item_Parent);
		var itemUi = item.GetComponent<ItemUI>();
		itemUi.Initialize(itemInformation);
		_items.Add(itemUi);
	}

	public void Refresh()
	{
		foreach (var item in _items)
		{
			item.Refresh();
		}
	}
}