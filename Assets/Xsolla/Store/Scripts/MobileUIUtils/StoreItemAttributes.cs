using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(StoreItemUI))]
    public class StoreItemAttributes : MonoBehaviour
    {
		[SerializeField] private GameObject AttributeItemPrefab = default;
		[SerializeField] private Transform AttributesRoot = default;

		private void Awake()
		{
			GetComponent<StoreItemUI>().OnInitialized += ShowAttributesIfAny;
		}

		private void ShowAttributesIfAny(CatalogItemModel itemModel)
		{
			var attributes = itemModel.Attributes;

			if (attributes == null)
				return;

			foreach (var attribute in attributes)
			{
				var newObject = Instantiate(AttributeItemPrefab, AttributesRoot);
				var uiScript = newObject.GetComponent<AttributeItem>();
				uiScript.Key = attribute.Key;
				uiScript.Value = attribute.Value;
			}
		}
	}
}
