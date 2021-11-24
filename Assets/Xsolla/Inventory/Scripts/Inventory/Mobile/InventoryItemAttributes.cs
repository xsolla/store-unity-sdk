using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class InventoryItemAttributes : MonoBehaviour
	{
		[SerializeField] private AttributeItem AttributeItemPrefab;
		[SerializeField] private Transform AttributesRoot;

		private void Awake()
		{
			GetComponent<InventoryItemUI>().OnInitialized += ShowAttributesIfAny;
		}

		private void ShowAttributesIfAny(ItemModel itemModel)
		{
			var attributes = itemModel.Attributes;

			foreach (Transform child in AttributesRoot)
			{
				Destroy(child.gameObject);
			}

			if (attributes == null)
				return;

			if (itemModel is InventoryItemModel)
			{
				var item = (InventoryItemModel)itemModel;
				if (item.RemainingUses != null)
					AddAttributeItem("Quantity:", item.RemainingUses.Value.ToString());
			}

			foreach (var attribute in attributes)
			{
				AddAttributeItem(attribute.Key + ":", attribute.Value);
			}
		}

		private void AddAttributeItem(string key, string value)
		{
			var newObject = Instantiate(AttributeItemPrefab, AttributesRoot);
			newObject.Key = key;
			newObject.Value = value;
			LayoutRebuilder.ForceRebuildLayoutImmediate(newObject.transform as RectTransform);
		}
	}
}
