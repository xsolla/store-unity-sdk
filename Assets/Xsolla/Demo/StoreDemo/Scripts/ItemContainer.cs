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

	public List<ItemUI> Items { get; private set; }

	private void Awake()
	{
		Items = new List<ItemUI>();
		DisableEmptyContainerMessage();
	}

	public void AddItem(Xsolla.Store.StoreItem itemInformation)
	{
		ItemUI item = Instantiate(itemPrefab, itemParent).GetComponent<ItemUI>();
		item.Initialize(itemInformation);
		Items.Add(item);
	}

	public void Refresh()
	{
		foreach (var item in Items)
		{
			item.Refresh();
		}
	}

	public void EnableEmptyContainerMessage()
	{
		emptyMessageText.gameObject.SetActive(true);
	}

	private void DisableEmptyContainerMessage()
	{
		emptyMessageText.gameObject.SetActive(false);
	}
}
