using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryItemInfoMobile : MonoBehaviour
	{
		[SerializeField] private GameObject FullscreenUI = default;

		private static InventoryItemInfoMobile _instance;

		string lastShownItemSku = default;

		public static void ShowItem(ItemModel itemModel)
		{
			if (_instance == null)
			{
				Debug.LogError("Instance not set");
				return;
			}

			_instance.ShowUI(true);
			_instance.lastShownItemSku = itemModel.Sku;
			_instance.FullscreenUI.GetComponent<InventoryItemUI>().Initialize(itemModel, DemoController.Instance.InventoryDemo);
		}

		private void Awake()
		{
			_instance = this;

			var closeFullscreenButton = FullscreenUI.GetComponent<ButtonProvider>()?.Button;
			if (closeFullscreenButton != null)
				closeFullscreenButton.onClick += () => ShowUI(false);

			UserInventory.Instance.RefreshEvent += () =>
			{
				if (_instance.FullscreenUI.activeSelf && !string.IsNullOrEmpty(_instance.lastShownItemSku))
				{
					var inventoryItem = UserInventory.Instance.AllItems.Find(item => item.Sku == lastShownItemSku);
					if (inventoryItem != null)
					{
						ShowItem(inventoryItem);
					}
					else
					{
						ShowUI(false);
					}
				}
			};
		}

		private void OnDestroy()
		{
			if (_instance == this)
				_instance = null;
		}

		private void ShowUI(bool fullscreen)
		{
			FullscreenUI.SetActive(fullscreen);
		}
	}
}