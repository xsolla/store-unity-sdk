using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public partial class InventoryItemUI : MonoBehaviour
	{
		partial void AttachRenewSubscriptionHandler()
		{
			if (renewSubscriptionButton == null)
				return;

			var subscriptionItem = UserCatalog.Instance.AllItems.First(s => s.Sku == _itemInformation.Sku);
			if (subscriptionItem.VirtualPrice == null)
				renewSubscriptionButton.onClick = () => DemoController.Instance.StoreDemo.PurchaseForRealMoney(subscriptionItem);
			else
				renewSubscriptionButton.onClick = () => DemoController.Instance.StoreDemo.PurchaseForVirtualCurrency(subscriptionItem);
		}
	}
}