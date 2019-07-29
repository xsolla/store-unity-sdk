using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public class GroupsController : MonoBehaviour
{
	[SerializeField]
	GameObject groupPrefab;
	[SerializeField]
	GameObject groupCartPrefab;

	[SerializeField]
	RectTransform scrollView;

	List<IGroup> _groups;
	
	ItemsController _itemsController;
	ItemsControls _itemsControls;
	
	void Awake()
	{
		_groups = new List<IGroup>();

		_itemsController = FindObjectOfType<ItemsController>();
		_itemsControls = FindObjectOfType<ItemsControls>();
	}

	public void CreateGroups(StoreItems items)
	{
		foreach (var item in items.items)
		{
			if (item.groups.Any())
			{
				foreach (var groupName in item.groups)
				{
					if (!_groups.Exists(group => group.Name == groupName))
					{
						AddGroup(groupPrefab, groupName);
					}
				}
			}
			else
			{
				if (!_groups.Exists(group => group.Name == Constants.UngroupedGroupName))
				{
					AddGroup(groupPrefab, Constants.UngroupedGroupName);
				}
			}
		}

		AddGroup(groupCartPrefab, Constants.CartGroupName);
	}

	void AddGroup(GameObject groupPref, string groupName)
	{
		var newGroup = Instantiate(groupPref, scrollView.transform).GetComponent<IGroup>();
		newGroup.Name  = groupName;
		newGroup.OnGroupClick += (id) =>
		{
			_itemsController.ActivateContainer(id);
			ChangeSelection(id);
			
			_itemsControls.SetStore();
		};

		_groups.Add(newGroup);
	}

	void ChangeSelection(string groupName)
	{
		foreach (var group in _groups)
		{
			if (group.Name != groupName)
			{
				group.Deselect();
			}
		}
	}

	public void SelectDefault()
	{
		if (_groups.Any())
		{
			_groups.First().Select();
		}
	}

	public string GetSelectedGroup()
	{
		var selectedGroup = _groups.Find((group => group.IsSelected()));
		
		return selectedGroup != null ? selectedGroup.Name : string.Empty;
	}
}