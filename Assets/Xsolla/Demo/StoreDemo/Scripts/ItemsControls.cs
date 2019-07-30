using UnityEngine;
using Xsolla.Core;

public class ItemsControls : MonoBehaviour
{
	[SerializeField]
	MenuButton storeButton;
	[SerializeField]
	MenuButton inventoryButton;

	public void Init()
	{
		var groupsController = FindObjectOfType<GroupsController>();
		var itemsController = FindObjectOfType<ItemsController>();
		
		storeButton.gameObject.SetActive(true);
		inventoryButton.gameObject.SetActive(true);
		
		storeButton.Select(false);
		
		storeButton.onClick = ((s) =>
		{
			inventoryButton.Deselect();

			var selectedGroup = groupsController.GetSelectedGroup();

			if (selectedGroup != null)
			{
				itemsController.ActivateContainer(selectedGroup.Name);
			}
		});
		
		inventoryButton.onClick = ((s) =>
		{
			storeButton.Deselect();
			
			itemsController.ActivateContainer(Constants.InventoryConatainerName);
		});
	}

	public void ActivateStoreTab(string groupID)
	{
		if (groupID != Constants.CartGroupName)
		{
			storeButton.gameObject.SetActive(true);
			inventoryButton.gameObject.SetActive(true);
			
			storeButton.Select(false);
			inventoryButton.Deselect();
		}
		else
		{
			storeButton.gameObject.SetActive(false);
			inventoryButton.gameObject.SetActive(false);
		}
	}
}