using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.DisplayCatalog
{
	public class VirtualItemsByGroupsPage : MonoBehaviour
	{
		// Declaration of variables for widget's container and prefab
		public Transform GroupButtonsContainer;
		public GameObject GroupButtonPrefab;
		public Transform ItemWidgetsContainer;
		public GameObject ItemWidgetPrefab;

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
			// Selecting the group’s name from items and order them alphabetical
			var groupNames = storeItems.items
				.SelectMany(x => x.groups)
				.GroupBy(x => x.name)
				.Select(x => x.First())
				.OrderBy(x => x.name)
				.Select(x => x.name)
				.ToList();

			// Add group name for “all groups”, which will mean show all items regardless of group affiliation
			groupNames.Insert(0, "All");

			// Iterating the group names collection
			foreach (var groupName in groupNames)
			{
				// Instantiating the button prefab as child of the container
				var buttonGo = Instantiate(GroupButtonPrefab, GroupButtonsContainer, false);
				var groupButton = buttonGo.GetComponent<VirtualItemGroupButton>();

				// Assigning the values for ui elements
				groupButton.NameText.text = groupName;

				// Adding listener for button click event
				groupButton.Button.onClick.AddListener(() => OnGroupSelected(groupName, storeItems));
			}

			// Calling method for redraw page
			OnGroupSelected("All", storeItems);
		}

		private void OnGroupSelected(string groupName, StoreItems storeItems)
		{
			// Clear container
			DeleteAllChildren(ItemWidgetsContainer);

			// Declaring variable for items which will display on page
			IEnumerable<StoreItem> itemsForDisplay;
			if (groupName == "All")
			{
				itemsForDisplay = storeItems.items;
			}
			else
			{
				itemsForDisplay = storeItems.items.Where(item => item.groups.Any(group => group.name == groupName));
			}

			// Iterating the items collection and assign values for appropriate ui elements
			foreach (var storeItem in itemsForDisplay)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(ItemWidgetPrefab, ItemWidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualItemWidget>();

				// Assigning the values for ui elements
				widget.NameText.text = storeItem.name;
				widget.DescriptionText.text = storeItem.description;

				// The item can be purchased for real money or virtual currency
				// Checking the price type and assign the appropriate value for price text
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

				// Loading the image for item icon and assign it to the ui element
				ImageLoader.LoadSprite(storeItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Some actions
		}

		// Utility method for delete all children of container
		private static void DeleteAllChildren(Transform parent)
		{
			var childList = parent.Cast<Transform>().ToList();
			foreach (var childTransform in childList)
			{
				Destroy(childTransform.gameObject);
			}
		}
	}
}