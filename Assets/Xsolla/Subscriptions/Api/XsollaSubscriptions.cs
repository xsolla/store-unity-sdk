using System;
using Xsolla.Core;

namespace Xsolla.Subscriptions
{
	public class XsollaSubscriptions : MonoSingleton<XsollaSubscriptions>
	{
		private const string BASE_SUBSCRIPTIONS_API_URL = "https://subscriptions.xsolla.com";

		private const string URL_GET_PUBLIC_PLANS = BASE_SUBSCRIPTIONS_API_URL + "/api/public/v1/projects/{0}/user_plans";

		private const string URL_GET_SUBSCRIPTIONS = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions";
		private const string URL_GET_SUBSCRIPTION_DETAILS = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions/{1}";
		private const string URL_CANCEL_SUBSCRIPTION = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions/{1}/cancel";
		private const string URL_GET_RENEWAL_URL = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions/{1}/renew";
		private const string URL_GET_PURCHASE_URL = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions/buy";
		private const string URL_GET_MANAGEMENT_URL = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/subscriptions/manage";

		private const string URL_GET_PLANS = BASE_SUBSCRIPTIONS_API_URL + "/api/user/v1/projects/{0}/plans";

		/// <summary>
		/// Returns a list of all plans, including plans purchased by the user while promotions are active.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{merplan_id}).</param>
		/// <param name="onSuccess">Called after public plans have been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		/// By default, it is determined by the user's IP address.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionPublicPlans(string projectId, Action<PlanItems> onSuccess, Action<Error> onError = null, int[] planId = null, string[] planExternalId = null, int limit = 50, int offset = 0, string locale = null, string country = null)
		{
			var url = string.Format(URL_GET_PUBLIC_PLANS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_id", planId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_external_id", planExternalId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, country: country);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, onSuccess, onError);
		}

		/// <summary>
		/// Returns a list of active recurrent subscriptions that have the status `active`, `non renewing`, and `pause`.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="onSuccess">Called after the list pf subscriptions has been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		/// By default, it is determined by the user's IP address.</param>
		public void GetSubscriptions(string projectId, Action<SubscriptionItems> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string locale = null)
		{
			var url = string.Format(URL_GET_SUBSCRIPTIONS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Returns information about a subscription by its ID. Subscription can be have any status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="subscriptionId">Subscription ID.</param>
		/// <param name="onSuccess">Called after subscription data have been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Language of the UI.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		/// By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionDetails(string projectId, int subscriptionId, Action<SubscriptionDetails> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var url = string.Format(URL_GET_SUBSCRIPTION_DETAILS, projectId, subscriptionId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Changes a regular subscription status to `non_renewing` (subscription is automatically canceled after expiration).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="subscriptionId">Subscription ID.</param>
		/// <param name="onSuccess">Called after successful subscription cancelling.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void CancelSubscription(string projectId, int subscriptionId, Action onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_CANCEL_SUBSCRIPTION, projectId, subscriptionId);
			WebRequestHelper.Instance.PutRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Returns the URL of the renewal interface for the selected subscription.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="subscriptionId">Subscription ID. </param>
		/// <param name="paymentSettings">Pay Station UI settings.</param>
		/// <param name="onSuccess">Called after the URL has been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetSubscriptionRenewalUrl(string projectId, int subscriptionId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_GET_RENEWAL_URL, projectId, subscriptionId);
			var data = new RenewalSubscriptionRequest(CheckAndFillPaymentSettings(settings));
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionRenewalUrl(string projectId, int subscriptionId, Action<PaymentLink> onSuccess, Action<Error> onError = null)
		{
			GetSubscriptionRenewalUrl(projectId, subscriptionId, new PaymentSettings(), onSuccess, onError);
		}

		/// <summary>
		/// Returns Pay Station URL for the subscription purchase.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="paymentSettings">Pay Station UI settings.</param>
		/// <param name="onSuccess">Called after the URL has been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionPurchaseUrl(string projectId, string planExternalId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			var url = string.Format(URL_GET_PURCHASE_URL, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, country: country);

			var data = new BuySubscriptionRequest(planExternalId, CheckAndFillPaymentSettings(settings));
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionPurchaseUrl(string projectId, string planExternalId, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			GetSubscriptionPurchaseUrl(projectId, planExternalId, new PaymentSettings(), onSuccess, onError, country);
		}

		/// <summary>
		/// Returns the URL of the management interface for the selected subscription.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="paymentSettings">Settings.</param>
		/// <param name="onSuccess">Called after the URL jas beeb successfully revieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="country">User's country. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionManagementUrl(string projectId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			var url = string.Format(URL_GET_MANAGEMENT_URL, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, country: country);

			var data = new ManageSubscriptionRequest(CheckAndFillPaymentSettings(settings));
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionManagementUrl(string projectId, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			GetSubscriptionManagementUrl(projectId, new PaymentSettings(), onSuccess, onError, country);
		}

		/// <summary>
		/// Returns a list of plans available to authorized users, including plans purchased by the user while promotions are active.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project.</param>
		/// <param name="onSuccess">Called after a list of plans has been successfully recieved.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{merplan_id}).</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<bt/>
		/// By default, it is determined by the user's IP address.</param>
		/// <param name="country">User's country. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionPlans(string projectId, Action<PlanItems> onSuccess, Action<Error> onError = null, int[] planId = null, string[] planExternalId = null, int limit = 50, int offset = 0, string locale = null, string country = null)
		{
			var url = string.Format(URL_GET_PLANS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_id", planId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_external_id", planExternalId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, country: country);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		private PaymentSettings CheckAndFillPaymentSettings(PaymentSettings paymentSettings)
		{
			if (paymentSettings == null)
				paymentSettings = new PaymentSettings();

			if (paymentSettings.sandbox == null)
				paymentSettings.sandbox = XsollaSettings.IsSandbox;

			var commonUiSettings = PayStationUISettings.GenerateSettings();
			if (commonUiSettings != null)
			{
				if (paymentSettings.ui == null)
					paymentSettings.ui = new PaymentSettings.UI();

				if (paymentSettings.ui.size == null)
					paymentSettings.ui.size = commonUiSettings.size;
				if (paymentSettings.ui.theme == null)
					paymentSettings.ui.theme = commonUiSettings.theme;
				if (paymentSettings.ui.version == null)
					paymentSettings.ui.version = commonUiSettings.version;
			}

			var commonRedirectPolicy = RedirectPolicySettings.GeneratePolicy();
			if (commonRedirectPolicy != null)
			{
				if (paymentSettings.redirect_policy == null)
					paymentSettings.redirect_policy = new PaymentSettings.RedirectPolicy();

				if (paymentSettings.return_url == null)
					paymentSettings.return_url = commonRedirectPolicy.return_url;
				if (paymentSettings.redirect_policy.redirect_conditions == null)
					paymentSettings.redirect_policy.redirect_conditions = commonRedirectPolicy.redirect_conditions;
				if (paymentSettings.redirect_policy.delay == null)
					paymentSettings.redirect_policy.delay = commonRedirectPolicy.delay;
				if (paymentSettings.redirect_policy.status_for_manual_redirection == null)
					paymentSettings.redirect_policy.status_for_manual_redirection = commonRedirectPolicy.status_for_manual_redirection;
				if (paymentSettings.redirect_policy.redirect_button_caption == null)
					paymentSettings.redirect_policy.redirect_button_caption = commonRedirectPolicy.redirect_button_caption;
			}

			return paymentSettings;
		}
	}
}
