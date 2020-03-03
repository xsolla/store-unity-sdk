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
	ItemsTabControl _itemsTabControl;
	
	void Awake()
	{
		_groups = new List<IGroup>();

		_itemsController = FindObjectOfType<ItemsController>();
		_itemsTabControl = FindObjectOfType<ItemsTabControl>();
	}

	private void Start()
	{
		GroupsHotKeys hotKeys = gameObject.GetComponent<GroupsHotKeys>();
		hotKeys.ArrowDownKeyPressedEvent += () => {
			IGroup group = GetSelectedGroup();
			int index = _groups.IndexOf(group) + 1;
			if (index >= _groups.Count)
				index = 0;
			group = _groups.ElementAt(index);
			group.Select();
			SelectGroup(group.Id);
		};
		hotKeys.ArrowUpKeyPressedEvent += () => {
			IGroup group = GetSelectedGroup();
			int index = _groups.IndexOf(group);
			if (index == 0) {
				index = _groups.Count - 1;
			} else {
				index--;
			}
			group = _groups.ElementAt(index);
			group.Select();
			SelectGroup(group.Id);
		};
	}

	public void CreateGroups(StoreItems items, Groups groups)
	{
		foreach (var item in items.items)
		{
			if (item.groups.Any())
			{
				foreach (var group in item.groups)
				{
					AddGroup(groupPrefab, group.name, group.name);
				}
			}
			else
			{
				AddGroup(groupPrefab, Constants.UngroupedGroupName, Constants.UngroupedGroupName);
			}
		}

		AddGroup(groupPrefab, Constants.CurrencyGroupName, Constants.CurrencyGroupName);
		AddGroup(groupCartPrefab, Constants.CartGroupName, Constants.CartGroupName);
	}

	void AddGroup(GameObject groupPref, string groupId, string groupName)
	{
		if (_groups.Exists(group => group.Id == groupId))
			return;
		var newGroup = Instantiate(groupPref, scrollView.transform).GetComponent<IGroup>();
		newGroup.Id = groupId;
		newGroup.Name  = groupName;
		newGroup.OnGroupClick += SelectGroup;

		_groups.Add(newGroup);
	}

	void ChangeSelection(string groupId)
	{
		foreach (var group in _groups)
		{
			if (group.Id != groupId)
			{
				group.Deselect();
			}
		}
	}

	private void SelectGroup(string id)
	{
		_itemsController.ActivateContainer(id);
		ChangeSelection(id);
		_itemsTabControl.ActivateStoreTab(id);
	}

	public void SelectDefault()
	{
		if (_groups.Any())
		{
			_groups.First().Select();
		}
	}

	public IGroup GetSelectedGroup()
	{
		return _groups.Find((group => group.IsSelected()));
	}
}