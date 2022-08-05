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
		/// <see cref="https://subscriptions.xsolla.com/api/doc/public#/User%20Plans/get_xsolla_subscription_apipublic_getsubscriptionsplans"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (`https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{merplan_id}`).</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI. By default, it is determined by the user's IP address. Can be enforced by using an ISO 639-1 code.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
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
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/get_xsolla_subscription_apiuser_getuseractivesubscriptions"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI. By default, it is determined by the user's IP address. Can be enforced by using an ISO 639-1 code.</param>
		public void GetSubscriptions(string projectId, Action<SubscriptionItems> onSuccess, Action<Error> onError = null, int limit = 50, int offset = 0, string locale = null)
		{
			var url = string.Format(URL_GET_SUBSCRIPTIONS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Returns information about a subscription by its ID. Subscription can be in any status.
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/get_xsolla_subscription_apiuser_getusersubscription"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="subscriptionId">Subscription ID. **Required**.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Language of the UI. By default, it is determined by the user's IP address. Can be enforced by using an ISO 639-1 code.</param>
		public void GetSubscriptionDetails(string projectId, int subscriptionId, Action<SubscriptionDetails> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var url = string.Format(URL_GET_SUBSCRIPTION_DETAILS, projectId, subscriptionId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Changes a regular subscription status to non_renewing (subscription is automatically canceled after expiration).
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/put_xsolla_subscription_apiuser_cancelusersubscription"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="subscriptionId">Subscription ID. **Required**.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CancelSubscription(string projectId, int subscriptionId, Action onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_CANCEL_SUBSCRIPTION, projectId, subscriptionId);
			WebRequestHelper.Instance.PutRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		/// <summary>
		/// Returns Pay Station URL for the subscription renewal.
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/post_xsolla_subscription_apiuser_renewusersubscription"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="subscriptionId">Subscription ID. **Required**.</param>
		/// <param name="paymentSettings">Settings.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetSubscriptionRenewalUrl(string projectId, int subscriptionId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null)
		{
			var url = string.Format(URL_GET_RENEWAL_URL, projectId, subscriptionId);
			var data = new RenewalSubscriptionRequest(settings);
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionRenewalUrl(string projectId, int subscriptionId, Action<PaymentLink> onSuccess, Action<Error> onError = null)
		{
			GetSubscriptionRenewalUrl(projectId, subscriptionId, new PaymentSettings(XsollaSettings.IsSandbox), onSuccess, onError);
		}

		/// <summary>
		/// Returns Pay Station URL for the subscription purchase.
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/post_xsolla_subscription_apiuser_buysubscription"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="paymentSettings">Settings.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionPurchaseUrl(string projectId, string planExternalId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			var url = string.Format(URL_GET_PURCHASE_URL, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, country: country);

			var data = new BuySubscriptionRequest(planExternalId, settings);
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionPurchaseUrl(string projectId, string planExternalId, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			GetSubscriptionPurchaseUrl(projectId, planExternalId, new PaymentSettings(XsollaSettings.IsSandbox), onSuccess, onError, country);
		}

		/// <summary>
		/// Returns Pay Station URL for the subscription management.
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/user#/Subscriptions/post_xsolla_subscription_apiuser_getmanagesubscriptionslink"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="paymentSettings">Settings.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionManagementUrl(string projectId, PaymentSettings settings, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			var url = string.Format(URL_GET_MANAGEMENT_URL, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, country: country);

			var data = new ManageSubscriptionRequest(settings);
			WebRequestHelper.Instance.PostRequest(SdkType.Subscriptions, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}

		public void GetSubscriptionManagementUrl(string projectId, Action<PaymentLink> onSuccess, Action<Error> onError = null, string country = null)
		{
			GetSubscriptionManagementUrl(projectId, new PaymentSettings(XsollaSettings.IsSandbox), onSuccess, onError, country);
		}

		/// <summary>
		/// Returns a list of all plans, including plans purchased by the user while promotions are active.
		/// </summary>
		/// <see cref="https://subscriptions.xsolla.com/api/doc/public#/User%20Plans/get_xsolla_subscription_apipublic_getsubscriptionsplans"/>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. **Required**.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (`https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{merplan_id}`).</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI. By default, it is determined by the user's IP address. Can be enforced by using an ISO 639-1 code.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public void GetSubscriptionPlans(string projectId, Action<PlanItems> onSuccess, Action<Error> onError = null, int[] planId = null, string[] planExternalId = null, int limit = 50, int offset = 0, string locale = null, string country = null)
		{
			var url = string.Format(URL_GET_PLANS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_id", planId);
			url = UrlParameterizer.ConcatUrlAndParams(url, "plan_external_id", planExternalId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, country: country);
			WebRequestHelper.Instance.GetRequest(SdkType.Subscriptions, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError);
		}
	}
}