using System;
using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryFinder : MonoBehaviour
    {
		public void FindInInventory(string skuOrName, int maxAttempts, Action<InventoryItemModel> onFound, Action onAbsence = null)
		{
			StartCoroutine(TryGetFromInventory(skuOrName, maxAttempts, onFound, onAbsence));
		}

		private IEnumerator TryGetFromInventory(string skuOrName, int maxAttempts, Action<InventoryItemModel> onFind, Action onAbsence = null)
		{
			var inventoryItem = default(InventoryItemModel);
			var counter = 0;

			do
			{
				yield return new WaitWhile(() => !UserInventory.Instance.IsUpdated);
				counter++;
				inventoryItem = GetInventoryItem(skuOrName);

				if (inventoryItem == null)
					UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);

			}while (inventoryItem == null && counter <= maxAttempts);

			if/*now*/(inventoryItem != null)
			{
				if (onFind != null)
					onFind.Invoke(inventoryItem);
			}
			else
			{
				if (onAbsence != null)
					onAbsence.Invoke();
			}
		}

		private InventoryItemModel GetInventoryItem(string skuOrName)
		{
			foreach (var item in UserInventory.Instance.VirtualItems)
				if (item.Sku == skuOrName || item.Name == skuOrName)
					return item;
			//else
			return null;
		}
	}
}
