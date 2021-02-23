using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public abstract class BaseAttributeManager : MonoBehaviour
	{
		public Action<List<string>> OnRemoveUserAttributes;
		public Action<List<UserAttribute>> OnUpdateUserAttributes;

		public abstract void Initialize(List<UserAttribute> userReadOnlyAttributes = null, List<UserAttribute> userCustomAttributes = null);

		protected void RaiseOnRemoveUserAttributes(List<string> attributesToRemove) => OnRemoveUserAttributes?.Invoke(attributesToRemove);
		protected void RaiseOnUpdateUserAttributes(List<UserAttribute> attributesToUpdate) => OnUpdateUserAttributes?.Invoke(attributesToUpdate);
	}
}
