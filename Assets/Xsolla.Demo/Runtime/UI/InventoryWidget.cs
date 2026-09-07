using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryWidget : MonoBehaviour
	{
		[field: SerializeField] private Transform WidgetsContainer { get; set; }
		[field: SerializeField] private InventoryWidgetItem InventoryWidgetItemPrefab { get; set; }

		private IInventoryService InventoryService => ServiceLocator.Resolve<IInventoryService>();

		private void Start()
		{
			WidgetsContainer.ClearChildren();

			var items = InventoryService.GetAllItems();
			foreach (var item in items)
			{
				if (item.Quantity <= 0)
					continue;

				var widgetItem = Instantiate(InventoryWidgetItemPrefab, WidgetsContainer);
				widgetItem.Construct(item);
			}
		}
	}
}