using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public class AttributesItemsContainer : MonoBehaviour, IContainer
{
	[SerializeField]
	GameObject attributeItemPrefab;
	
	[SerializeField]
	GameObject attributesControlsPrefab;
	
	[SerializeField]
	Transform itemParent;
	
	List<GameObject> _attributeItems;
	GameObject _attributesControls;

	List<UserAttribute> _attributes;
	List<string> _attributesToRemove;

	void Awake()
	{
		_attributeItems = new List<GameObject>();
		_attributes = new List<UserAttribute>();
		_attributesToRemove =new List<string>();
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
		var attributeItemUi = newItem.GetComponent<AttributeItemUI>();
		attributeItemUi.Initialize(itemInformation);
		attributeItemUi.onRemove = OnRemoveAttribute;
		_attributeItems.Add(newItem);
	}
	
	void AddControls()
	{
		_attributesControls = Instantiate(attributesControlsPrefab, itemParent);
		AttributesControls controls = _attributesControls.GetComponent<AttributesControls>();
		controls.OnNewAttribute = OnNewAttribute;
		controls.OnSaveAttributes = OnSaveAttributes;
	}

	public void Refresh()
	{
		ClearAttributeItems();
		_attributes.Clear();
		_attributesToRemove.Clear();
		
		XsollaLogin.Instance.GetUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, null, null, list =>
		{
			_attributes = list;
			AddAttributes();
			AddControls();
		}, print);
	}

	void RefreshUiOnly()
	{
		ClearAttributeItems();
		AddAttributes();
		AddControls();
	}

	void ClearAttributeItems()
	{
		foreach (var item in _attributeItems)
		{
			Destroy(item);
		}
		
		_attributeItems.Clear();
		
		Destroy(_attributesControls);
	}

	void OnNewAttribute()
	{
		_attributes.Add(new UserAttribute());

		RefreshUiOnly();
	}
	
	void OnSaveAttributes()
	{
		XsollaLogin.Instance.UpdateUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, _attributes, () =>
		{
			print("READY UpdateUserAttributes");
		}, print);
		
		XsollaLogin.Instance.RemoveUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, _attributesToRemove, (() =>
		{
			print("READY RemoveUserAttributes");
		}), print);
	}

	void OnRemoveAttribute(UserAttribute attributeToRemove)
	{
		if (!_attributes.Remove(attributeToRemove))
		{
			print("Something went wrong - can't remove attribute");
		}

		MarkAttributeToRemove(attributeToRemove.key);
		
		RefreshUiOnly();
	}

	public void MarkAttributeToRemove(string key)
	{
		if (!_attributesToRemove.Contains(key))
		{
			_attributesToRemove.Add(key);
		}
	}

	public bool ContainsAttribute(string key)
	{
		return _attributes.Find(item => item.key == key) != null;
	}
}