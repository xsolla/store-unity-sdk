using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

[RequireComponent(typeof(AttributeItemsManager))]
public class AttributeItemsManagerDebug : MonoBehaviour
{
	private List<UserAttribute> _readonlyAttributes = new List<UserAttribute>
	{
		new UserAttribute() {key = "Email", value = "supprot@xsolla.com"},
		new UserAttribute() {key = "Username", value = "Support"}
	};

	private List<UserAttribute> _customAttributes = new List<UserAttribute>
	{
		new UserAttribute() {key = "Level", value = "80"},
		new UserAttribute() {key = "Panic", value = "none"}
	};

	private void Awake()
	{
		var manager = GetComponent<AttributeItemsManager>();

		manager.Initialize(_readonlyAttributes, _customAttributes);

		manager.OnRemoveUserAttributes = OnRemoveAttributes;
		manager.OnUpdateUserAttributes = OnUpdateAttributes;
	}

	private	void OnRemoveAttributes(List<string> attributeKeys)
	{
		foreach (var attributeKey in attributeKeys)
			Debug.Log(string.Format("AttributeItemsManagerDebug: Will be removed: '{0}'", attributeKey));
	}

	private void OnUpdateAttributes(List<UserAttribute> attributes)
	{
		foreach (var attribute in attributes)
			Debug.Log(string.Format("AttributeItemsManagerDebug: Will be modified: Key:{0} Value:{1}", attribute.key, attribute.value));
	}
}
