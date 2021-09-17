using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class UserAttributesProvider : UserAttributesRequestBase
	{
		[SerializeField] private BaseAttributeManager ItemsManager = default;
		[SerializeField] private UserAttributeType AttributeType = default;

		public void ProvideUserAttributes()
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

			SdkLoginLogic.Instance.GetUserAttributes(base.Token, base.ProjectID, this.AttributeType, attributeKeys, userId, onSuccessGet, base.OnError);
		}
	}
}
