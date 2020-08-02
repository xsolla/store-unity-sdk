using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

public class UserAttributesProvider : UserAttributesRequestBase
{
#pragma warning disable 0649
	[SerializeField] private AttributeItemsManager ItemsManager;
	[SerializeField] private UserAttributeType AttributeType;
#pragma warning restore 0649

	private void Start()
	{
		if (!base.IsRequestPossible)
			return;

		List<string> attributeKeys = null;//Get all existing attributes without filter
		string userId = null;//Get current user attributes

		Action<List<UserAttribute>> onSuccessGet = result =>
		{
			switch (AttributeType)
			{
				case UserAttributeType.CUSTOM:
					ItemsManager.Initialize(userCustomAttributes: result);
					break;
				case UserAttributeType.READONLY:
					ItemsManager.Initialize(userReadOnlyAttributes: result);
					break;
			}

			base.OnSuccess?.Invoke();
		};

		XsollaLogin.Instance.GetUserAttributes(base.Token, base.ProjectID, this.AttributeType, attributeKeys, userId, onSuccessGet, base.OnError);
	}
}
