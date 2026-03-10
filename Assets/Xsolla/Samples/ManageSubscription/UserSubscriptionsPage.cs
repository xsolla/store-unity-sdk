using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Samples.ManageSubscription
{
	public class UserSubscriptionsPage : MonoBehaviour
	{
		// Declaration of variables for widget's container and prefab
		public Transform WidgetsContainer;
		public GameObject WidgetPrefab;

		private void Start()
		{
			// Starting the authentication process
			// Pass the credentials and callback functions for success and error cases
			// The credentials (username and password) are hard-coded for simplicity
			XsollaAuth.SignIn("xsolla", "xsolla", OnAuthenticationSuccess, OnError);
		}

		private void OnAuthenticationSuccess()
		{
			// Starting the request for the user's active subscriptions after successful authentication
			// Pass the callback functions for success and error cases
			XsollaSubscriptions.GetSubscriptions(OnSubscriptionsRequestSuccess, OnError);
		}

		private void OnSubscriptionsRequestSuccess(SubscriptionItems subscriptionItems)
		{
			// Iterating the subscriptions collection
			foreach (var subscriptionItem in subscriptionItems.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<UserSubscriptionWidget>();

				// Assigning the values for UI elements
				widget.NameText.text = subscriptionItem.plan_name;
				widget.StatusText.text = subscriptionItem.status;

				// Assigning the next charge date if available
				widget.NextChargeDateText.text = subscriptionItem.date_next_charge.HasValue
					? subscriptionItem.date_next_charge.Value.ToShortDateString()
					: "—";

				// Assigning the charge amount if available
				if (subscriptionItem.charge != null)
					widget.PriceText.text = $"{subscriptionItem.charge.amount} {subscriptionItem.charge.currency}";

				// Assigning the click listener for the manage button
				widget.ManageButton.onClick.AddListener(() =>
				{
					// Starting the management URL request
					// Pass the callback functions for success and error cases
					XsollaSubscriptions.GetUserAccountUrl(OnManagementUrlReceived, OnError);
				});
			}
		}

		private void OnManagementUrlReceived(UserAccountLink accountLink)
		{
			// Opening the subscription management URL in the browser
			XsollaWebBrowser.Open(accountLink.redirect_url);
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
