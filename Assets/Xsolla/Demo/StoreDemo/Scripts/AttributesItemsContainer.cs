using System.Collections.Generic;
using System.Linq;
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
	
	StoreController _storeController;
	
	List<GameObject> _attributeItems;
	GameObject _attributesControls;

	List<UserAttribute> _attributes;
	List<string> _attributesToRemove;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		
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

		_attributesToRemove.Clear();

		_attributes = CopyAttributes(_storeController.attributes);
		
		AddAttributes();
		AddControls();
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
			XsollaLogin.Instance.RemoveUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, _attributesToRemove, (() =>
			{
				StoreDemoPopup.ShowSuccess();
				_storeController.RefreshAttributes(Refresh);
			}), StoreDemoPopup.ShowError);
		}, StoreDemoPopup.ShowError);
	}

	void OnRemoveAttribute(UserAttribute attributeToRemove)
	{
		if (!_attributes.Remove(attributeToRemove))
		{
			Debug.LogError("Something went wrong - can't remove attribute");
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

	static List<UserAttribute> CopyAttributes(List<UserAttribute> attributes)
	{
		return attributes.Select(item => item.GetCopy()).ToList();
	}
}