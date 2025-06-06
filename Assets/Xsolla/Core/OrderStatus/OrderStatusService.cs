using System;

namespace Xsolla.Core
{
	internal static class OrderStatusService
	{
		public static void GetOrderStatus(int orderId, Action<OrderStatus> onSuccess, Action<Error> onError, SdkType sdkType = SdkType.Store)
		{
			if (OrderStatusCache.TryPerform(orderId, onSuccess))
				return;

			PerformWebRequest(
				orderId,
				status =>
				{
					OrderStatusCache.UpdateStatus(status);
					onSuccess?.Invoke(status);
				},
				onError,
				sdkType);
		}

		private static void PerformWebRequest(int orderId, Action<OrderStatus> onSuccess, Action<Error> onError, SdkType sdkType)
		{
			var url = $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}/order/{orderId}";

			WebRequestHelper.Instance.GetRequest(
				sdkType,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => PerformWebRequest(orderId, onSuccess, onError, sdkType)),
				ErrorGroup.OrderStatusErrors);
		}
	}
}