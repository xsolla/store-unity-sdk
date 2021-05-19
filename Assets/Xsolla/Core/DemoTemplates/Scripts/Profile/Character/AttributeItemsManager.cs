using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Login;

public class AttributeItemsManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] GameObject AttributeItemPrefab;
	[SerializeField] Transform CustomAttributesParentTransform;
	[SerializeField] Transform ReadOnlyAttributesParentTransform;
	[SerializeField] SimpleButton NewButton;
	[SerializeField] SimpleButton SaveButton;
#pragma warning restore 0649

	private const string CUSTOM_ATTRIBUTE_KEY = "CustomAttribute";
	private const string CUSTOM_ATTRIBUTE_VALUE = "Custom value";

	private List<AttributeItem> _readOnlyAttributes;
	private Dictionary<string, AttributeItem> _customAttributes;
	private List<string> _removedAttributes;
	private bool _isInitialized;
	private bool _isAlive;

	public Action<List<string>> OnRemoveUserAttributes;
	public Action<List<UserAttribute>> OnUpdateUserAttributes;

	private void Awake()
	{
		_isAlive = true;
		NewButton.onClick += AddNewAttribute;
		SaveButton.onClick += SaveAttributes;

		AttributeItem.OnKeyChanged += HandleKeyChanged;
		AttributeItem.OnValueChanged += HandleValueChanged;
		AttributeItem.OnRemoveRequest += HandleRemoveRequest;

		_removedAttributes = new List<string>();
	}

	private void OnDestroy()
	{
		_isAlive = false;
		AttributeItem.OnKeyChanged -= HandleKeyChanged;
		AttributeItem.OnValueChanged -= HandleValueChanged;
		AttributeItem.OnRemoveRequest -= HandleRemoveRequest;
	}

	public void Initialize(List<UserAttribute> userReadOnlyAttributes = null, List<UserAttribute> userCustomAttributes = null)
	{
		if(!_isAlive)//This method may be called after user switched to another page, this is a workaround to this problem
			return;

		if (userReadOnlyAttributes != null)
			_readOnlyAttributes = InstantiateReadOnlyAttributes(userReadOnlyAttributes);

		if (userCustomAttributes != null)
			_customAttributes = InstantiateCustomAttributes(userCustomAttributes);

		if (_readOnlyAttributes == null)
			_readOnlyAttributes = new List<AttributeItem>();

		if (_customAttributes == null)
			_customAttributes = new Dictionary<string, AttributeItem>();

		_isInitialized = true;
	}

	private List<AttributeItem> InstantiateReadOnlyAttributes(List<UserAttribute> userAttributes)
	{
		var result = new List<AttributeItem>(capacity: userAttributes.Count);

		foreach (var attribute in userAttributes)
		{
			var attributeItem = InstantiateAttribute(attribute, isReadOnly: true, parentTransform: ReadOnlyAttributesParentTransform);
			result.Add(attributeItem);
		}

		return result;
	}

	private Dictionary<string, AttributeItem> InstantiateCustomAttributes(List<UserAttribute> userAttributes)
	{
		var result = new Dictionary<string, AttributeItem>(capacity: userAttributes.Count);

		foreach (var attribute in userAttributes)
		{
			var attributeItem = InstantiateAttribute(attribute, isReadOnly: false, parentTransform: CustomAttributesParentTransform);
			result[attributeItem.Key] = attributeItem;
		}

		return result;
	}

	private AttributeItem InstantiateAttribute(UserAttribute userAttribute, bool isReadOnly, Transform parentTransform)
	{
		var attributeItemObject = Instantiate<GameObject>(AttributeItemPrefab, parentTransform);
		var attributeItem = attributeItemObject.GetComponent<AttributeItem>();

		attributeItem.IsReadOnly = isReadOnly;
		attributeItem.IsPublic = userAttribute.permission != "private";
		attributeItem.Key = userAttribute.key;
		attributeItem.Value = userAttribute.value;

		return attributeItem;
	}

	private void HandleKeyChanged(AttributeItem attributeItem, string oldKey, string newKey)
	{
		if (attributeItem.IsReadOnly)
		{
			Debug.LogError(string.Format("AttributeItemsManager.HandleKeyChanged: Attempt to change key of read-only attribute. OldKey:{0} NewKey:{1}", oldKey, newKey));
			attributeItem.Key = oldKey;
		}
		else if (newKey.Length > 256 || !Regex.IsMatch(newKey, "^[A-Za-z0-9_]+$"))
		{
			Debug.Log(string.Format("AttributeItemsManager.HandleKeyChanged: New key does not follow rules MaxLength:256 Pattern:[A-Za-z0-9_]+ OldKey:{0} NewKey:{1}", oldKey, newKey));
			attributeItem.Key = oldKey;
		}
		else if (_customAttributes.ContainsKey(newKey))
		{
			Debug.Log(string.Format("AttributeItemsManager.HandleKeyChanged: This key already exists. Key:{0}", newKey));
			attributeItem.Key = oldKey;
		}
		else
		{
			if (!attributeItem.IsNew && !_removedAttributes.Contains(oldKey))
				_removedAttributes.Add(oldKey);

			if (_removedAttributes.Contains(newKey))
				_removedAttributes.Remove(newKey);

			_customAttributes.Remove(oldKey);
			_customAttributes[newKey] = attributeItem;
			
			attributeItem.IsModified = true;
		}
	}

	private void HandleValueChanged(AttributeItem attributeItem, string oldValue, string newValue)
	{
		if (attributeItem.IsReadOnly)
		{
			Debug.LogError(string.Format("AttributeItemsManager.HandleValueChanged: Attempt to change value of read-only attribute. Key:{0} OldValue:{1} NewValue:{2}", attributeItem.Key, oldValue, newValue));
			attributeItem.Value = oldValue;
		}
		else if (newValue.Length > 256)
		{
			Debug.Log(string.Format("AttributeItemsManager.HandleValueChanged: New value does not follow rule MaxLength:256 Key:{0} OldValue:{1} NewValue:{2}", attributeItem.Key, oldValue, newValue));
			attributeItem.Value = oldValue;
		}
		else
		{
			attributeItem.IsModified = true;
		}
	}

	private void HandleRemoveRequest(AttributeItem attributeItem)
	{
		if (attributeItem.IsReadOnly)
		{
			Debug.LogError(string.Format("AttributeItemsManager.HandleRemoveRequest: Attempt to remove read-only attribute. Key:{0} Value:{1}", attributeItem.Key, attributeItem.Value));
		}
		else
		{
			var key = attributeItem.Key;

			if (!attributeItem.IsNew && !_removedAttributes.Contains(key))
				_removedAttributes.Add(key);

			_customAttributes.Remove(key);
			Destroy(attributeItem.gameObject);
		}
	}

	private void AddNewAttribute()
	{
		if (!_isInitialized)
			return;

		var customCount = 0;
		var customKey = CUSTOM_ATTRIBUTE_KEY;

		while (_customAttributes.ContainsKey(customKey))
		{
			customCount++;
			customKey = string.Format("{0}_{1}", CUSTOM_ATTRIBUTE_KEY, customCount);
		}

		var attributeItemObject = Instantiate<GameObject>(AttributeItemPrefab, CustomAttributesParentTransform);
		var attributeItem = attributeItemObject.GetComponent<AttributeItem>();

		attributeItem.IsReadOnly = false;
		attributeItem.IsPublic = true;
		attributeItem.Key = customKey;
		attributeItem.Value = CUSTOM_ATTRIBUTE_VALUE;
		
		attributeItem.IsNew = true;
		_customAttributes[customKey] = attributeItem;
	}

	private void SaveAttributes()
	{
		if (!_isInitialized)
			return;

		if (_removedAttributes.Count > 0)
		{
			if (OnRemoveUserAttributes != null)
				OnRemoveUserAttributes.Invoke(_removedAttributes);
			_removedAttributes.Clear();
		}

		var modifiedAttributes = new List<UserAttribute>();
		var garbageAttributeItems = new List<AttributeItem>();

		foreach (var pair in _customAttributes)
		{
			var attribute = pair.Value;
			
			if (attribute.IsNew && !attribute.IsModified)
			{
				garbageAttributeItems.Add(attribute);
			}
			else if (attribute.IsModified)
			{
				attribute.IsModified = false;
				attribute.IsNew = false;
				
				var packedAttribute = new UserAttribute();
				packedAttribute.key = attribute.Key;
				packedAttribute.permission = attribute.IsPublic ? "public" : "private";
				packedAttribute.value = attribute.Value;

				modifiedAttributes.Add(packedAttribute);
			}
		}

		foreach (var garbage in garbageAttributeItems)
		{
			_customAttributes.Remove(garbage.Key);
			Destroy(garbage.gameObject);
		}

		if (modifiedAttributes.Count > 0)
		{
			if (OnUpdateUserAttributes != null)
				OnUpdateUserAttributes.Invoke(modifiedAttributes);
		}
	}
}
