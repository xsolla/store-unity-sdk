namespace Xsolla.Demo
{
	public class CatalogServiceFactory
	{
		public ICatalogService Create()
		{
			var catalogStorage = new CatalogStorage();
			return new XsollaCatalogService(catalogStorage);
		}
	}
}