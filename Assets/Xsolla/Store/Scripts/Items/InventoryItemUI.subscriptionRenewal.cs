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
				renewSubscriptionButton.onClick = () => DemoShop.Instance.PurchaseForRealMoney(subscriptionItem);
			else
				renewSubscriptionButton.onClick = () => DemoShop.Instance.PurchaseForVirtualCurrency(subscriptionItem);
		}
	}
}