using System.Collections;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class PurchaseScenario : Scenario
	{
		protected abstract string ItemSku { get; }
		protected virtual PlatformSpecificAppearance Appearance => null;
		protected virtual bool ClosesInAppBrowserAfterPurchase => false;

		protected IEnumerator PurchaseItem()
		{
			Error purchaseError = null;

			yield return WaitFor<OrderStatus>(
				(onSuccess, onError) => XsollaCatalog.Purchase(ItemSku, onSuccess, onError, platformSpecificAppearance: Appearance),
				_ => { },
				error => purchaseError = error);

			if (ClosesInAppBrowserAfterPurchase)
				CloseInAppBrowser();

			if (purchaseError != null)
				TryCompleteError(purchaseError);
			else
				TryCompleteSuccess();
		}
	}
}