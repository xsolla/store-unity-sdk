using System;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Subscriptions
{
	public partial class XsollaSubscriptions : MonoSingleton<XsollaSubscriptions>
	{
		private const string URL_GET_SUBSCRIPTIONS = Constants.BASE_STORE_API_URL + "/user/subscriptions";

		/// <summary>
		/// Retrieves the current user’s subscriptions.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's subscriptions</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-subscriptions"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		public void GetSubscriptions(string projectId, [NotNull] Action<SubscriptionItems> onSuccess, [CanBeNull] Action<Error> onError, string platform = null)
		{
			var url = string.Format(URL_GET_SUBSCRIPTIONS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetSubscriptions(projectId, onSuccess, onError, platform)),
				ErrorCheckType.ItemsListErrors);
		}
	}
}
