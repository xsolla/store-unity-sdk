using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Samples.DisplayItemsInInventory
{
	public class InventoryItemsPage : MonoBehaviour
	{
		// Declaration of variables for widget's container and prefab
		public Transform WidgetsContainer;
		public GameObject WidgetPrefab;

		private void Start()
		{
			// Starting the authentication process
			// Pass the credentials and callback functions for success and error cases
			// The credentials (username and password) are hardcoded for the sake of simplicity
			XsollaAuth.SignIn("xsolla", "xsolla", OnAuthenticationSuccess, OnError);
		}

		private void OnAuthenticationSuccess()
		{
			// After successful authentication starting the request for inventory items
			// Pass the callback functions for success and error cases
			XsollaInventory.GetInventoryItems(OnItemsRequestSuccess, OnError);
		}

		private void OnItemsRequestSuccess(InventoryItems inventoryItems)
		{
			// Iterating the items collection
			foreach (var inventoryItem in inventoryItems.items)
			{
				// Skipping virtual currency items
				if (inventoryItem.VirtualItemType == VirtualItemType.VirtualCurrency)
					continue;

				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<InventoryItemWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = inventoryItem.name;
				widget.DescriptionText.text = inventoryItem.description;
				widget.QuantityText.text = inventoryItem.quantity.ToString();

				// Loading the item image and assign it to the ui element
				ImageLoader.LoadSprite(inventoryItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
		}
	}
}