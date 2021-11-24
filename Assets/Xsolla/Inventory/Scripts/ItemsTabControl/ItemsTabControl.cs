using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class ItemsTabControl : MonoBehaviour
	{
		[SerializeField] private MenuButton storeButton;
		[SerializeField] private MenuButton inventoryButton;

		private ItemsController _itemsController;
		private GroupsController _groupsController;

		public void Init()
		{
			_groupsController = FindObjectOfType<GroupsController>();
			_itemsController = FindObjectOfType<ItemsController>();

			storeButton.gameObject.SetActive(true);
			inventoryButton.gameObject.SetActive(true);

			storeButton.Select(false);
			storeButton.onClick = _ => InternalActivateStoreTab();
			inventoryButton.onClick = _ => InternalActivateInventoryTab();

			InitHotKeys();
		}

		private void InternalActivateStoreTab()
		{
			inventoryButton.Deselect();

			var selectedGroup = _groupsController.GetSelectedGroup();
			if (selectedGroup != null)
			{
				_itemsController.ActivateContainer(selectedGroup.Id);
			}
		}

		private void InternalActivateInventoryTab()
		{
			storeButton.Deselect();
			_itemsController.ActivateContainer(StoreConstants.INVENTORY_CONTAINER_NAME);
		}

		public void ActivateStoreTab()
		{
			storeButton.Select(false);
			inventoryButton.Deselect();
		}
	}
}
