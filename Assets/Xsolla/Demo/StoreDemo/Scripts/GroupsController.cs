using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public class GroupsController : MonoBehaviour
{
	[SerializeField]
	GameObject _groupPrefab;
	[SerializeField]
	GameObject _groupCartPrefab;

	[SerializeField]
	RectTransform _scrollView;

	List<GameObject> _groups;
	
	ItemsController itemsController;

	void Awake()
	{
		_groups = new List<GameObject>();

		itemsController = FindObjectOfType<ItemsController>();
	}

	public void CreateGroups(StoreItems items)
	{
		var addedGroups = new List<string>();
		
		foreach (var item in items.items)
		{
			if (item.groups.Any())
			{
				foreach (string groupName in item.groups)
				{
					if (!addedGroups.Contains(groupName))
					{
						addedGroups.Add(groupName);
						AddGroup(groupName);
					}
				}
			}
			else
			{
				if (!addedGroups.Contains(Constants.UngroupedGroupName))
				{
					addedGroups.Add(Constants.UngroupedGroupName);
					AddGroup(Constants.UngroupedGroupName);
				}
			}
		}

		AddCart();
	}

	public void AddGroup(string groupName)
	{
		GameObject newGroup = Instantiate(_groupPrefab, _scrollView.transform);
		newGroup.GetComponent<GroupUI>().InitializeGroup(groupName);
		newGroup.GetComponent<GroupUI>().OnGroupClick += (id) =>
		{
			itemsController.ActivateContainer(id);
			ChangeSelection(id);
		};

		_groups.Add(newGroup);
	}

	public void AddCart()
	{
		GameObject newGroup = Instantiate(_groupCartPrefab, _scrollView.transform);
		newGroup.GetComponent<CartGroupUI>().onGroupClick += (id) =>
		{
			itemsController.ActivateContainer(id);
			ChangeSelection(id);
		};

		_groups.Add(newGroup);
	}

	public void ChangeSelection(string groupName)
	{
		foreach (var group in _groups)
		{
			var groupComponent = group.GetComponent<GroupUI>();
			if (groupComponent != null)
			{
				groupComponent.Deselect(groupName);
			}
			else
			{
				var cartGroupComponent = group.GetComponent<CartGroupUI>();
				if (cartGroupComponent != null)
				{
					cartGroupComponent.Deselect(groupName);
				}
			}
		}
	}
}