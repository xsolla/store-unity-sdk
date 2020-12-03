using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

public class UserAttributesUpdater : UserAttributesRequestBase
{
#pragma warning disable 0649
	[SerializeField] private AttributeItemsManager ItemsManager;
#pragma warning restore 0649

	private void Awake()
	{
		ItemsManager.OnRemoveUserAttributes = RemoveAttributes;
		ItemsManager.OnUpdateUserAttributes = UpdateAttributes;
	}

	private void RemoveAttributes(List<string> attributeKeys)
	{
		if (!base.IsRequestPossible)
			return;

		DemoController.Instance.GetImplementation().RemoveUserAttributes(base.Token, base.ProjectID, attributeKeys, base.OnSuccess, base.OnError);
	}

	private void UpdateAttributes(List<UserAttribute> updatedAttributes)
	{
		if (!base.IsRequestPossible)
			return;

		DemoController.Instance.GetImplementation().UpdateUserAttributes(base.Token, base.ProjectID, updatedAttributes, base.OnSuccess, base.OnError);
	}
}
