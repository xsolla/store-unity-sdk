using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;

public abstract class StoreController : MonoBehaviour
{
	GroupsController _groupsController;
	ItemsController _itemsController;
	ItemsTabControl _itemsTabControl;

	protected abstract IDemoImplementation GetImplementation();

	protected virtual void OnDestroy()
	{
		StopAllCoroutines();
		if (UserCatalog.IsExist)
			Destroy(UserCatalog.Instance.gameObject);
		if (UserInventory.IsExist)
			Destroy(UserInventory.Instance.gameObject);
	}

	protected virtual void Start()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();
		_itemsTabControl = FindObjectOfType<ItemsTabControl>();

		ApplyStoreImplementation(GetImplementation());
		CatalogInit();
	}

	private void ApplyStoreImplementation(IDemoImplementation implementation)
	{
		_itemsController.Init(implementation);
		UserCatalog.Instance.Init(implementation);
		UserInventory.Instance.Init(implementation);
	}

	private void CatalogInit()
	{
		_groupsController.GroupSelectedEvent += groupId =>
		{
			_itemsController.ActivateContainer(groupId);
			_itemsTabControl.ActivateStoreTab();
		};
		UserCatalog.Instance.UpdateItems(InitStoreUi);
	}

	protected virtual void InitStoreUi()
	{
		// This method used for fastest async image loading
		StartLoadItemImages(UserCatalog.Instance.AllItems);

		_itemsTabControl.Init();

		CreateAndFillCatalogGroups(UserCatalog.Instance.AllItems);

		RefreshInventory();
	}

	private void StartLoadItemImages(List<CatalogItemModel> items)
	{
		items.ForEach(i => ImageLoader.Instance.GetImageAsync(i.ImageUrl, null));
	}

	private void CreateAndFillCatalogGroups(List<CatalogItemModel> items)
	{
		if (items.Any())
		{
			items.ForEach(i =>
			{
				var groups = GetImplementation().GetCatalogGroupsByItem(i);
				groups.ForEach(groupName =>
				{
					_groupsController.AddGroup(groupName);
					_itemsController.AddItemToContainer(groupName, i);
				});
			});
		}
		else
		{
			_groupsController.AddGroup("ALL");
		}

		_groupsController.SelectDefault();
	}

	private void RefreshInventory()
	{
		UserInventory.Instance.Refresh();
	}
}