using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Store;

public class OldAttributesItemsContainer : MonoBehaviour
{
	[SerializeField]
	GameObject attributeItemPrefab;
	
	[SerializeField]
	OldAttributesControls AttributesControls;
	
	[SerializeField]
	Transform AttributesParentTransform;

	private List<GameObject> _attributeItems;
	private GameObject _attributesControls;

	private List<OldUserAttributeModel> _attributes;
	private List<string> _attributesToRemove;

	private void Awake()
	{
		_attributeItems = new List<GameObject>();
		_attributes = new List<OldUserAttributeModel>();
		_attributesToRemove = new List<string>();
	}

	private OldUserAttributeModel GetAttribute(string key) => _attributes.First(a => a.key.Equals(key));
	
	private void OnAttributeKeyChanged(OldAttributeItemUI attribute, string oldKey)
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
	
	private void OnAttributeValueChanged(OldAttributeItemUI attribute)
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

	private void OnRemoveAttribute(OldAttributeItemUI attributeToRemove)
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

	private void Refresh(List<OldUserAttributeModel> attributes)
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
	
	private void AddAttributeItem(OldUserAttributeModel itemInformation)
	{
		GameObject newItem = Instantiate(attributeItemPrefab, AttributesParentTransform);
		var attributeItemUi = newItem.GetComponent<OldAttributeItemUI>();
		attributeItemUi.Initialize(itemInformation.key, itemInformation.value);
		attributeItemUi.onKeyChanged = OnAttributeKeyChanged;
		attributeItemUi.onValueChanged = OnAttributeValueChanged;
		attributeItemUi.onRemove = OnRemoveAttribute;
		_attributeItems.Add(newItem);
	}

	private void AddControls()
	{
		AttributesControls.OnNewAttribute = OnNewAttribute;
		AttributesControls.OnSaveAttributes = OnSaveAttributes;
	}
	
	void OnNewAttribute()
	{
		_attributes.Add(new OldUserAttributeModel());
		RefreshUiOnly();
	}
	
	void OnSaveAttributes()
	{
		Action callback = () =>
		{
			StoreDemoPopup.ShowSuccess();
			OldUserAttributes.Instance.GetAttributes(Refresh, StoreDemoPopup.ShowError);
		};
		if (_attributesToRemove.Any())
			OldUserAttributes.Instance.UpdateAttributes(_attributes, _attributesToRemove, callback, StoreDemoPopup.ShowError);
		else
			OldUserAttributes.Instance.SetAttributes(_attributes, callback, StoreDemoPopup.ShowError);
	}

	private void ChangeAttributeKey(OldUserAttributeModel model, string newKey)
	{
		RemoveAttribute(model.key);
		model.key = newKey;
		CancelRemovingAttribute(newKey);
	}

	private void RemoveAttribute(string key)
	{
		if (!OldUserAttributes.Instance.Attributes.Exists(a => a.key.Equals(key)))
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