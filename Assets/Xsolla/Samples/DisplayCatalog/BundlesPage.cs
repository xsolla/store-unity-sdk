using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.DisplayCatalog
{
	public class BundlesPage : MonoBehaviour
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
			// After successful authentication starting the request for bundles from store
			// Pass the callback functions for success and error cases
			XsollaCatalog.GetBundles(OnBundlesRequestSuccess, OnError);
		}

		private void OnBundlesRequestSuccess(BundleItems bundleItems)
		{
			// Iterating the bundles collection
			foreach (var bundleItem in bundleItems.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<BundleWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = bundleItem.name;
				widget.DescriptionText.text = bundleItem.description;

				// Creating the string with bundle content and assign it to the ui element
				var bundleContent = bundleItem.content.Select(x => $"{x.name} x {x.quantity}");
				widget.ContentText.text = string.Join("\n", bundleContent);

				// The bundle can be purchased for real money or virtual currency
				// Checking the price type and assign the values for appropriate ui elements
				if (bundleItem.price != null)
				{
					var realMoneyPrice = bundleItem.price;
					widget.PriceText.text = $"{realMoneyPrice.amount} {realMoneyPrice.currency}";
				}
				else if (bundleItem.virtual_prices != null)
				{
					var virtualCurrencyPrice = bundleItem.virtual_prices.First(x => x.is_default);
					widget.PriceText.text = $"{virtualCurrencyPrice.name}: {virtualCurrencyPrice.amount}";
				}

				// Loading the bundle image and assign it to the ui element
				ImageLoader.LoadSprite(bundleItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Some actions
		}
	}
}