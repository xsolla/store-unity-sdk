using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.DisplayCatalog
{
	public class VirtualCurrencyPackagesPage : MonoBehaviour
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
			// After successful authentication starting the request for packages from store
			// Pass the callback functions for success and error cases
			XsollaCatalog.GetVirtualCurrencyPackagesList(OnPackagesRequestSuccess, OnError);
		}

		private void OnPackagesRequestSuccess(VirtualCurrencyPackages packageItems)
		{
			// Iterating the virtual currency packages collection
			foreach (var packageItem in packageItems.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualCurrencyPackageWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = packageItem.name;
				widget.DescriptionText.text = packageItem.description;

				// The package can be purchased for real money or virtual currency
				// Checking the price type and assign the values for appropriate ui elements
				if (packageItem.price != null)
				{
					var realMoneyPrice = packageItem.price;
					widget.PriceText.text = $"{realMoneyPrice.amount} {realMoneyPrice.currency}";
				}
				else if (packageItem.virtual_prices != null)
				{
					var virtualCurrencyPrice = packageItem.virtual_prices.First(x => x.is_default);
					widget.PriceText.text = $"{virtualCurrencyPrice.name}: {virtualCurrencyPrice.amount}";
				}

				// Loading the package image and assign it to the ui element
				ImageLoader.LoadSprite(packageItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Some actions
		}
	}
}