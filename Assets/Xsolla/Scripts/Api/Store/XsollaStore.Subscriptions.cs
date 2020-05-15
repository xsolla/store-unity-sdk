using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_GET_SUBSCRIPTIONS = "https://store.xsolla.com/api/v2/project/{0}/user/subscriptions";

		/// <summary>
		/// Retrieves the current user’s subscriptions.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's subscriptions</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-subscriptions"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetInventoryItems(string projectId, [NotNull] Action<Subscriptions> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_GET_SUBSCRIPTIONS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}
	}
}