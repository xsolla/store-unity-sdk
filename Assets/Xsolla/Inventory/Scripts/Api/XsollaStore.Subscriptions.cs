using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_GET_SUBSCRIPTIONS = BASE_STORE_API_URL + "/user/subscriptions";

		/// <summary>
		/// Retrieves the current user’s subscriptions.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's subscriptions</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-subscriptions"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetSubscriptions(string projectId, [NotNull] Action<SubscriptionItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_GET_SUBSCRIPTIONS, projectId);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, platformParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}
	}
}
