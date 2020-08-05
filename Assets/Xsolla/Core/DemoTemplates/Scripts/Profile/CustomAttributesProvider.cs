using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

[RequireComponent(typeof(AttributeItemsManager))]
public class CustomAttributesProvider : StoreActionResult
{
	private void Start()
	{
		var itemsManager = GetComponent<AttributeItemsManager>();

		string token = XsollaLogin.Instance.Token;
		string projectID = XsollaSettings.StoreProjectId;
		List<string> attributeKeys = null;//Get all existing attributes without filter
		string userId = null;//Get current user attributes

		Action<List<UserAttribute>> onSuccessLoad = result =>
		{
			itemsManager.Initialize(userCustomAttributes: result);
			base.OnSuccess?.Invoke();
		};

		XsollaLogin.Instance.GetUserAttributes(token, projectID, attributeKeys, userId, onSuccessLoad, base.OnError);
	}

	private List<UserAttribute> GetUserCustomAttributes()
	{
		List<UserAttribute> result = null;


		Debug.Log($"Result == null: {result == null}");
		return result;
	}
}
