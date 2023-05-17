using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.SellForVirtualCurrency
{
	public class VirtualItemsPage : MonoBehaviour
	{
		// Declaration of variables for containers and widget prefabs
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
				// Skipping the items without price for virtual currency
				if (storeItem.virtual_prices.Length == 0)
					continue;

				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualItemWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = storeItem.name;
				widget.DescriptionText.text = storeItem.description;

				// Assigning the price and currency values for ui elements
				// The first price is used for the sake of simplicity
				var price = storeItem.virtual_prices.First(x => x.is_default);
				widget.PriceText.text = $"{price.name}: {price.amount}";

				// Loading the item image and assign it to the ui element
				ImageLoader.LoadSprite(storeItem.image_url, sprite => widget.IconImage.sprite = sprite);

				// Assigning the click listener for the buy button
				widget.BuyButton.onClick.AddListener(() =>
				{
					// Starting the purchase process
					// Pass the item sku, price sku and callback functions for success and error cases
					XsollaCatalog.PurchaseForVirtualCurrency(storeItem.sku, price.sku, OnPurchaseSuccess, OnError);
				});
			}
		}

		private void OnPurchaseSuccess(OrderStatus status)
		{
			Debug.Log("Purchase successful");
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
		}
	}
}