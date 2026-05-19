using System.Collections;

namespace Xsolla.Demo
{
	public interface ICatalogService
	{
		IEnumerator FetchCatalogCoroutine();

		CatalogRecord GetItem(string sku);
	}
}