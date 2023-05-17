using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.SellForRealMoney
{
	public class VirtualItemsPage : MonoBehaviour
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
			// After successful authentication starting the request for catalog from store
			// Pass the callback functions for success and error cases
			XsollaCatalog.GetCatalog(OnItemsRequestSuccess, OnError);
		}

		private void OnItemsRequestSuccess(StoreItems storeItems)
		{
			// Iterating the items collection
			foreach (var storeItem in storeItems.items)
			{
				// Skipping the items without price for real money
				if (storeItem.price == null)
					continue;

				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualItemWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = storeItem.name;
				widget.DescriptionText.text = storeItem.description;

				// Assigning the price and currency values for ui elements
				var price = storeItem.price;
				widget.PriceText.text = $"{price.amount} {price.currency}";

				// Loading the item image and assign it to the ui element
				ImageLoader.LoadSprite(storeItem.image_url, sprite => widget.IconImage.sprite = sprite);

				// Assigning the click listener for the buy button
				widget.BuyButton.onClick.AddListener(() =>
				{
					// Starting the purchase process
					// Pass the item sku and callback functions for success and error cases
					XsollaCatalog.Purchase(storeItem.sku, OnPurchaseSuccess, OnError);
				});
			}
		}

		private void OnPurchaseSuccess(OrderStatus status)
		{
			Debug.Log("Purchase successful");
			// Some actions
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Some actions
		}
	}
}