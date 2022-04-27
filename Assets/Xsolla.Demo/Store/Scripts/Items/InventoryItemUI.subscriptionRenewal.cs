using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public partial class InventoryItemUI : MonoBehaviour
	{
		partial void AttachRenewSubscriptionHandler()
		{
			var subscriptionItem = UserCatalog.Instance.Subscriptions.FirstOrDefault(s => s.Sku == _itemInformation.Sku);
			if (subscriptionItem == null)
			{
				_canRenewSubscription = false;
				return;
			}

			if (!renewSubscriptionButton)
				return;

			if (subscriptionItem.VirtualPrice == null)
				renewSubscriptionButton.onClick = () => DemoShop.Instance.PurchaseForRealMoney(subscriptionItem);
			else
				renewSubscriptionButton.onClick = () => DemoShop.Instance.PurchaseForVirtualCurrency(subscriptionItem);
		}
	}
}