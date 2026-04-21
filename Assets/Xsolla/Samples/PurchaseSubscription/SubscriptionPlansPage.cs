using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Samples.PurchaseSubscription
{
	public class SubscriptionPlansPage : MonoBehaviour
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
			// Starting the request for public subscription plans after successful authentication
			// Pass the callback functions for success and error cases
			XsollaSubscriptions.GetSubscriptionPublicPlans(OnPlansRequestSuccess, OnError);
		}

		private void OnPlansRequestSuccess(PlanItems planItems)
		{
			// Iterating the plans collection
			foreach (var planItem in planItems.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<SubscriptionPlanWidget>();

				// Assigning the values for UI elements
				widget.NameText.text = planItem.plan_name;
				widget.DescriptionText.text = planItem.plan_description;

				// Assigning the price value for the UI element
				if (planItem.charge != null)
					widget.PriceText.text = $"{planItem.charge.amount} {planItem.charge.currency}";

				// Assigning the billing period value for the UI element
				if (planItem.period != null)
					widget.PeriodText.text = $"{planItem.period.value} {planItem.period.unit}";

				// Assigning the click listener for the buy button
				// Capturing the plan external ID for use in the lambda
				var planExternalId = planItem.plan_external_id;
				widget.BuyButton.onClick.AddListener(() =>
				{
					// Starting the purchase URL request for the selected plan
					// Pass the plan external ID and callback functions for success and error cases
					XsollaSubscriptions.GetSubscriptionPurchaseUrl(planExternalId, OnPurchaseUrlReceived, OnError);
				});
			}
		}

		private void OnPurchaseUrlReceived(PaymentLink paymentLink)
		{
			// Opening the Pay Station URL in the browser to complete the subscription purchase
			XsollaWebBrowser.Open(paymentLink.link_to_ps);
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
