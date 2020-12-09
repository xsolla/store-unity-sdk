using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class UserAttributesUpdater : UserAttributesRequestBase
	{
		[SerializeField] private AttributeItemsManager ItemsManager = default;

		private void Awake()
		{
			ItemsManager.OnRemoveUserAttributes = RemoveAttributes;
			ItemsManager.OnUpdateUserAttributes = UpdateAttributes;
		}

		private void RemoveAttributes(List<string> attributeKeys)
		{
			if (!base.IsRequestPossible)
				return;

			DemoController.Instance.LoginDemo.RemoveUserAttributes(base.Token, base.ProjectID, attributeKeys, base.OnSuccess, base.OnError);
		}

		private void UpdateAttributes(List<UserAttribute> updatedAttributes)
		{
			if (!base.IsRequestPossible)
				return;

			DemoController.Instance.LoginDemo.UpdateUserAttributes(base.Token, base.ProjectID, updatedAttributes, base.OnSuccess, base.OnError);
		}
	}
}
