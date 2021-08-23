using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>
	{
		private TutorialManager _tutorialManager;
		public TutorialManager TutorialManager => _tutorialManager;

		partial void InitInventory()
		{
			if (InventoryDemo != null && StoreDemo == null)
				_tutorialManager = GetComponent<TutorialManager>();

			if (_tutorialManager != null)
				IsTutorialAvailable = true;
		}

		partial void UpdateInventory()
		{
			if (!UserInventory.IsExist)
				UserInventory.Instance.Init(InventoryDemo);

			if (StoreDemo == null)
			{
				UserCatalog.Instance.Init(InventoryDemo);
				UserCatalog.Instance.UpdateItems(
					onSuccess: () => UserInventory.Instance.Refresh(),
					onError: error =>
					{
						Debug.LogError($"InventorySDK init failure: {error}");
						StoreDemoPopup.ShowError(error);
					});
			}
		}

		partial void DestroyInventory()
		{
			if (UserInventory.IsExist)
				Destroy(UserInventory.Instance.gameObject);
		}

		partial void AutoStartTutorial()
		{
			if (_tutorialManager != null && !IsAccessTokenAuth)
			{
				if (!_tutorialManager.IsTutorialCompleted())
				{
					_tutorialManager.ShowTutorial();
				}
				else
				{
					Debug.Log("Skipping tutorial since it was already completed.");
				}
			}
			else
			{
				Debug.Log("Tutorial is not available for this demo.");
			}
		}

		partial void ManualStartTutorial(bool showWelcomeMessage)
		{
			TutorialManager.ShowTutorial(showWelcomeMessage);
		}

		private static void StartLoadItemImages(List<CatalogItemModel> items)
		{
			items.ForEach(i =>
			{
				if (!string.IsNullOrEmpty(i.ImageUrl))
				{
					ImageLoader.Instance.GetImageAsync(i.ImageUrl, null);
				}
				else
				{
					Debug.LogError($"Catalog item with sku = '{i.Sku}' without image!");
				}
			});
		}
	}
}
