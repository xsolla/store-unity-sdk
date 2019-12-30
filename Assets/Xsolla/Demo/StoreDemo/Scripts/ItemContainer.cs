using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject itemPrefab;

	[SerializeField]
	Transform itemParent;

	[SerializeField]
	Text emptyMessageText;

	List<ItemUI> _items;

	void Awake()
	{
		_items = new List<ItemUI>();
		DisableEmptyContainerMessage();
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

	public void EnableEmptyContainerMessage()
	{
		emptyMessageText.gameObject.SetActive(true);
	}

	public void DisableEmptyContainerMessage()
	{
		emptyMessageText.gameObject.SetActive(false);
	}
}
