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
			
			itemsController.ActivateContainer(groupsController.GetSelectedGroup());
		});
		
		inventoryButton.onClick = ((s) =>
		{
			storeButton.Deselect();
			
			itemsController.ActivateContainer(Constants.InventoryConatainerName);
		});
	}

	public void SetStore()
	{
		storeButton.Select(false);
		inventoryButton.Deselect();
	}
}