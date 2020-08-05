using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Login;

public class AttributeItemsManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] GameObject AttributeItemPrefab;
	[SerializeField] Transform AttributesParentTransform;
	[SerializeField] SimpleButton NewButton;
	[SerializeField] SimpleButton SaveButton;
#pragma warning restore 0649

	private const string CUSTOM_ATTRIBUTE_KEY = "CustomAttribute";
	private const string CUSTOM_ATTRIBUTE_VALUE = "Custom value";

	private Dictionary<string, AttributeItem> _readOnlyAttributes;
	private Dictionary<string, AttributeItem> _customAttributes;
	private List<string> _removedAttributes;
	private bool _isInitialized;

	public Action<List<string>> OnRemoveUserAttributes;
	public Action<List<UserAttribute>> OnUpdateUserAttributes;

	private void Awake()
	{
		NewButton.onClick = AddNewAttribute;
		SaveButton.onClick = SaveAttributes;

		AttributeItem.OnKeyChanged += HandleKeyChanged;
		AttributeItem.OnValueChanged += HandleValueChanged;
		AttributeItem.OnRemoveRequest += HandleRemoveRequest;

		_removedAttributes = new List<string>();
	}

	private void OnDestroy()
	{
		AttributeItem.OnKeyChanged -= HandleKeyChanged;
		AttributeItem.OnValueChanged -= HandleValueChanged;
		AttributeItem.OnRemoveRequest -= HandleRemoveRequest;
	}

	public void Initialize(List<UserAttribute> userReadOnlyAttributes = null, List<UserAttribute> userCustomAttributes = null)
	{
		if (userReadOnlyAttributes != null)
			_readOnlyAttributes = InstantiateAndPackAttributes(userReadOnlyAttributes, isReadOnly: true);

		if (userCustomAttributes != null)
			_customAttributes = InstantiateAndPackAttributes(userCustomAttributes, isReadOnly: false);

		if (_readOnlyAttributes == null)
			_readOnlyAttributes = new Dictionary<string, AttributeItem>();

		if (_customAttributes == null)
			_customAttributes = new Dictionary<string, AttributeItem>();

		_isInitialized = true;
	}

	private Dictionary<string, AttributeItem> InstantiateAndPackAttributes(List<UserAttribute> userAttributes, bool isReadOnly)
	{
		var result = new Dictionary<string, AttributeItem>(capacity: userAttributes.Count);

		foreach (var attribute in userAttributes)
		{
			var attributeItemObject = Instantiate<GameObject>(AttributeItemPrefab, AttributesParentTransform);
			var attributeItem = attributeItemObject.GetComponent<AttributeItem>();
			
			attributeItem.IsReadOnly = isReadOnly;
			attributeItem.IsPublic = attribute.permission != "private";
			attributeItem.Key = attribute.key;
			attributeItem.Value = attribute.value;

			var newKey = attribute.key;

			if (result.ContainsKey(newKey))
				Debug.LogError($"AttributeItemsManager.InstantiateAndPackAttributes: Duplicated key. Key:{newKey}");

			result[newKey] = attributeItem;
		}

		return result;
	}

	private void HandleKeyChanged(AttributeItem attributeItem, string oldKey, string newKey)
	{
		if (attributeItem.IsReadOnly)
		{
			Debug.LogError($"AttributeItemsManager.HandleKeyChanged: Attempt to change key of read-only attribute. OldKey:{oldKey} NewKey:{newKey}");
			attributeItem.Key = oldKey;
		}
		else if (newKey.Length > 256 || !Regex.IsMatch(newKey, "^[A-Za-z0-9_]+$"))
		{
			Debug.Log($"AttributeItemsManager.HandleKeyChanged: New key does not follow rules MaxLength:256 Pattern:[A-Za-z0-9_]+ OldKey:{oldKey} NewKey:{newKey}");
			attributeItem.Key = oldKey;
		}
		else if (_readOnlyAttributes.ContainsKey(newKey) || _customAttributes.ContainsKey(newKey))
		{
			Debug.Log($"AttributeItemsManager.HandleKeyChanged: This key already exists. Key:{newKey}");
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
			Debug.LogError($"AttributeItemsManager.HandleValueChanged: Attempt to change value of read-only attribute. Key:{attributeItem.Key} OldValue:{oldValue} NewValue:{newValue}");
			attributeItem.Value = oldValue;
		}
		else if (newValue.Length > 256)
		{
			Debug.Log($"AttributeItemsManager.HandleValueChanged: New value does not follow rule MaxLength:256 Key:{attributeItem.Key} OldValue:{oldValue} NewValue:{newValue}");
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
			Debug.LogError($"AttributeItemsManager.HandleRemoveRequest: Attempt to remove read-only attribute. Key:{attributeItem.Key} Value:{attributeItem.Value}");
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
			customKey = $"{CUSTOM_ATTRIBUTE_KEY}_{customCount}";
		}

		var attributeItemObject = Instantiate<GameObject>(AttributeItemPrefab, AttributesParentTransform);
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
			OnRemoveUserAttributes?.Invoke(_removedAttributes);
			_removedAttributes.Clear();
		}

		var modifiedAttributes = new List<UserAttribute>();
		var garbageAttributes = new List<AttributeItem>();

		foreach (var pair in _customAttributes)
		{
			var attribute = pair.Value;
			
			if (attribute.IsNew && !attribute.IsModified)
			{
				garbageAttributes.Add(attribute);
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

		foreach (var garbage in garbageAttributes)
		{
			_customAttributes.Remove(garbage.Key);
			Destroy(garbage.gameObject);
		}

		if (modifiedAttributes.Count > 0)
			OnUpdateUserAttributes?.Invoke(modifiedAttributes);
	}
}
