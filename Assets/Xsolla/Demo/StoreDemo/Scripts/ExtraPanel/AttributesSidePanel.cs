using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

public class AttributesSidePanel : MonoBehaviour
{
	[SerializeField]
	GameObject attributeItemPrefab;

	[SerializeField]
	Transform itemParent;

	List<GameObject> _items;

	StoreController _storeController;

	List<UserAttribute> _attributes;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		_items = new List<GameObject>();
	}

	void AddAttributes()
	{
		foreach (var attribute in _attributes)
		{
			AddAttributeItem(attribute);
		}
	}

	void AddAttributeItem(UserAttribute itemInformation)
	{
		GameObject newItem = Instantiate(attributeItemPrefab, itemParent);
		var attributeItemUi = newItem.GetComponent<AttributesSidePanelItem>();
		attributeItemUi.Initialize(itemInformation);
		_items.Add(newItem);
	}

	void ClearItems()
	{
		foreach (var item in _items)
		{
			Destroy(item);
		}

		_items.Clear();
	}

	public void SetAttributes(List<UserAttribute> attributes)
	{
		ClearItems();

		_attributes = attributes;

		AddAttributes();
	}
}