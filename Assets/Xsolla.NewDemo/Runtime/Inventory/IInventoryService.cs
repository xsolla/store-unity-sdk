using System.Collections;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public interface IInventoryService
	{
		IEnumerator FetchInventoryCoroutine();

		IEnumerable<InventoryRecord> GetAllItems();

		InventoryRecord GetItem(string sku);
	}
}