using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>
	{
		partial void UpdateStore()
		{
			if (!UserCatalog.IsExist)
				UserCatalog.Instance.Init(InventoryDemo);
			UserCatalog.Instance.UpdateItems(() =>
			{
				if (UserInventory.IsExist)
				{
					UserInventory.Instance.Refresh();
					// This method used for fastest async image loading
					StartLoadItemImages(UserCatalog.Instance.AllItems);
				}
			});
		}

		partial void DestroyStore()
		{
			if (UserCatalog.IsExist)
				Destroy(UserCatalog.Instance.gameObject);
			if (UserSubscriptions.IsExist)
				Destroy(UserSubscriptions.Instance.gameObject);
			if (UserCart.IsExist)
				Destroy(UserCart.Instance.gameObject);
		}
	}
}
