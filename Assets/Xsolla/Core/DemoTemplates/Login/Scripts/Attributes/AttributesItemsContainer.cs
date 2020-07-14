using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Store;

public class AttributesItemsContainer : MonoBehaviour
{
	[SerializeField]
	GameObject attributeItemPrefab;
	
	[SerializeField]
	GameObject attributesControlsPrefab;
	
	[SerializeField]
	Transform itemParent;

	private List<GameObject> _attributeItems;
	private GameObject _attributesControls;

	private List<UserAttributeModel> _attributes;
	private List<string> _attributesToRemove;

	private void Awake()
	{
		_attributeItems = new List<GameObject>();
		_attributes = new List<UserAttributeModel>();
		_attributesToRemove = new List<string>();
	}

	private UserAttributeModel GetAttribute(string key) => _attributes.First(a => a.key.Equals(key));
	
	private void OnAttributeKeyChanged(AttributeItemUI attribute, string oldKey)
	{
		var model = GetAttribute(attribute.Key);
		if (model != null)
		{
			Debug.LogError($"Attribute with key = '{attribute.Key}' already exists. Change key back to '{oldKey}'");
			attribute.Key = oldKey;
			return;
		}
		model = GetAttribute(oldKey);
		if (model == null)
		{
			Debug.LogError($"Attribute with key = '{oldKey}' not found. Change key back to '{oldKey}'");
			attribute.Key = oldKey;
			return;
		}
		ChangeAttributeKey(model, attribute.Key);
	}
	
	private void OnAttributeValueChanged(AttributeItemUI attribute)
	{
		var model = GetAttribute(attribute.Key);
		if (model == null)
		{
			Debug.LogError($"Attribute with key = '{attribute.Key}' not found. Change key back to '{attribute.Key}'");
			attribute.Value = string.Empty;
			return;
		}
		model.value = attribute.Value;
	}

	private void OnRemoveAttribute(AttributeItemUI attributeToRemove)
	{
		var model = _attributes.First(a => a.key.Equals(attributeToRemove.Key));
		if (model == null)
			Debug.LogError($"Attribute with key = '{attributeToRemove.Key}' not found.");
		else
		{
			_attributes.Remove(model);
			RemoveAttribute(model.key);
		}
		RefreshUiOnly();
	}

	private void Refresh(List<UserAttributeModel> attributes)
	{
		_attributesToRemove.Clear();
		_attributes = attributes;
		RefreshUiOnly();
	}

	private void RefreshUiOnly()
	{
		ClearAttributeItems();
		AddAttributes();
		AddControls();
	}
	
	private void ClearAttributeItems()
	{
		_attributeItems.ForEach(Destroy);
		_attributeItems.Clear();
		Destroy(_attributesControls);
	}
	
	private void AddAttributes()
	{
		_attributes.ForEach(AddAttributeItem);
	}
	
	private void AddAttributeItem(UserAttributeModel itemInformation)
	{
		GameObject newItem = Instantiate(attributeItemPrefab, itemParent);
		var attributeItemUi = newItem.GetComponent<AttributeItemUI>();
		attributeItemUi.Initialize(itemInformation.key, itemInformation.value);
		attributeItemUi.onKeyChanged = OnAttributeKeyChanged;
		attributeItemUi.onValueChanged = OnAttributeValueChanged;
		attributeItemUi.onRemove = OnRemoveAttribute;
		_attributeItems.Add(newItem);
	}

	private void AddControls()
	{
		_attributesControls = Instantiate(attributesControlsPrefab, itemParent);
		AttributesControls controls = _attributesControls.GetComponent<AttributesControls>();
		controls.OnNewAttribute = OnNewAttribute;
		controls.OnSaveAttributes = OnSaveAttributes;
	}
	
	void OnNewAttribute()
	{
		_attributes.Add(new UserAttributeModel());
		RefreshUiOnly();
	}
	
	void OnSaveAttributes()
	{
		Action callback = () =>
		{
			StoreDemoPopup.ShowSuccess();
			UserAttributes.Instance.GetAttributes(Refresh, StoreDemoPopup.ShowError);
		};
		if (_attributesToRemove.Any())
			UserAttributes.Instance.UpdateAttributes(_attributes, _attributesToRemove, callback, StoreDemoPopup.ShowError);
		else
			UserAttributes.Instance.SetAttributes(_attributes, callback, StoreDemoPopup.ShowError);
	}

	private void ChangeAttributeKey(UserAttributeModel model, string newKey)
	{
		RemoveAttribute(model.key);
		model.key = newKey;
		CancelRemovingAttribute(newKey);
	}

	private void RemoveAttribute(string key)
	{
		if (!UserAttributes.Instance.Attributes.Exists(a => a.key.Equals(key)))
			CancelRemovingAttribute(key);
		else
			_attributesToRemove.Add(key);
	}

	private void CancelRemovingAttribute(string key)
	{
		if (_attributesToRemove.Contains(key))
			_attributesToRemove.Remove(key);
	}
}