using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	public interface IStoreListener
	{
		void OnAuthSuccess();

		void OnAuthError(Error error);

		void OnAuthCancelled();

		void OnGetCatalogSuccess();

		void OnGetCatalogError(Error error);

		void OnPurchaseSuccess(OrderStatus orderStatus);

		void OnPurchaseError(Error error);
	}
}