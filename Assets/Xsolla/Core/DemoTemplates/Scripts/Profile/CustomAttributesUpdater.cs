using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

[RequireComponent(typeof(AttributeItemsManager))]
public class CustomAttributesUpdater : StoreActionResult
{
	private string Token => XsollaLogin.Instance.Token;
	private string ProjectID => XsollaSettings.StoreProjectId;

	private void Awake()
	{
		var manager = GetComponent<AttributeItemsManager>();
		
		manager.OnRemoveUserAttributes = RemoveAttributes;
		manager.OnUpdateUserAttributes = UpdateAttributes;
	}

	private void RemoveAttributes(List<string> attributeKeys)
	{
		XsollaLogin.Instance.RemoveUserAttributes(Token, ProjectID, attributeKeys, base.OnSuccess, base.OnError);
	}

	private void UpdateAttributes(List<UserAttribute> updatedAttributes)
	{
		XsollaLogin.Instance.UpdateUserAttributes(Token, ProjectID, updatedAttributes, base.OnSuccess, base.OnError);
	}
}
