using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Utilities;
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
	List<string> _attributesToremove;

	void Awake()
	{
		_attributeItems = new List<GameObject>();
		_attributes = new List<UserAttribute>();
		_attributesToremove =new List<string>();
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
		var attributeItemUI = newItem.GetComponent<AttributeItemUI>();
		attributeItemUI.Initialize(itemInformation);
		attributeItemUI.onRemove = OnRemoveAttribute;
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
		_attributesToremove.Clear();
		
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
		// _attributes won't work, need to get actual list items data
		XsollaLogin.Instance.UpdateUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, _attributes, () =>
		{
			print("READY UpdateUserAttributes");
		}, print);
		
		XsollaLogin.Instance.RemoveUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, _attributesToremove, (() =>
		{
			print("READY RemoveUserAttributes");
		}), print);
	}

	void OnRemoveAttribute(string key)
	{
		if (!_attributesToremove.Contains(key))
		{
			_attributesToremove.Add(key);
		}
		
		RefreshUiOnly();
	}
}