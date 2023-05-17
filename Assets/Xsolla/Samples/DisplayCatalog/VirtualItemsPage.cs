using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.DisplayCatalog
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
			XsollaCatalog.GetCatalog(OnItemsRequestSuccess, OnError);
		}

		private void OnItemsRequestSuccess(StoreItems storeItems)
		{
			// Iterating the items collection
			foreach (var storeItem in storeItems.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualItemWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = storeItem.name;
				widget.DescriptionText.text = storeItem.description;

				// The item can be purchased for real money or virtual currency
				// Checking the price type and assign the values for appropriate ui elements
				if (storeItem.price != null)
				{
					var realMoneyPrice = storeItem.price;
					widget.PriceText.text = $"{realMoneyPrice.amount} {realMoneyPrice.currency}";
				}
				else if (storeItem.virtual_prices != null)
				{
					var virtualCurrencyPrice = storeItem.virtual_prices.First(x => x.is_default);
					widget.PriceText.text = $"{virtualCurrencyPrice.name}: {virtualCurrencyPrice.amount}";
				}

				// Loading the item image and assign it to the ui element
				ImageLoader.LoadSprite(storeItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Some actions
		}
	}
}