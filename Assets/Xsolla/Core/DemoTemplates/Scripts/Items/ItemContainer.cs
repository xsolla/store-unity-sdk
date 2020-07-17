using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour
{
	[SerializeField] Transform itemParent;
	[SerializeField] Text emptyMessageText;

	private readonly List<GameObject> _items = new List<GameObject>();

	private void Awake()
	{
		DisableEmptyContainerMessage();
	}

	public bool IsEmpty()
	{
		return _items.Count == 0;
	}

	public GameObject AddItem(GameObject itemPrefab)
	{
		var itemObject = Instantiate(itemPrefab, itemParent);
		_items.Add(itemObject);
		return itemObject;
	}

	public void RemoveItem(GameObject itemForRemove)
	{
		if (!_items.Contains(itemForRemove)) return;
		_items.Remove(itemForRemove);
		Destroy(itemForRemove);
	}

	public void Clear()
	{
		_items.ForEach(Destroy);
		_items.Clear();
	}

	public void EnableEmptyContainerMessage(string text = null)
	{
		emptyMessageText.gameObject.SetActive(true);
		if (!string.IsNullOrEmpty(text))
			emptyMessageText.text = text;
	}

	private void DisableEmptyContainerMessage()
	{
		emptyMessageText.gameObject.SetActive(false);
	}
}