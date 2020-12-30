using System.Collections.Generic;
using UnityEngine;
using Xsolla.Login;

namespace Xsolla.Demo
{
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
				Debug.Log($"AttributeItemsManagerDebug: Will be removed: '{attributeKey}'");
		}

		private void OnUpdateAttributes(List<UserAttribute> attributes)
		{
			foreach (var attribute in attributes)
				Debug.Log($"AttributeItemsManagerDebug: Will be modified: Key:{attribute.key} Value:{attribute.value}");
		}
	}
}
