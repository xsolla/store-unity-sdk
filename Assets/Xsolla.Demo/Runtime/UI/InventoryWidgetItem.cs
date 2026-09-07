using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class InventoryWidgetItem : MonoBehaviour
	{
		[field: SerializeField] private Image IconImage { get; set; }
		[field: SerializeField] private TMP_Text QuantityText { get; set; }

		public void Construct(InventoryRecord record)
		{
			IconImage.sprite = record.IconSprite;

			var quantity = record.Quantity;
			QuantityText.text = quantity.ToString();
			QuantityText.gameObject.SetActive(quantity > 1);
		}
	}
}