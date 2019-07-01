using System;
using System.Collections.Generic;
using UnityEngine;

public class GroupsController : MonoBehaviour
{
	[SerializeField]
	GameObject _groupPrefab;
	[SerializeField]
	GameObject _groupCartPrefab;

	[SerializeField]
	RectTransform _scrollView;

	public Action<string> OnGroupClick;

	List<GameObject> _groups;

	void Awake()
	{
		_groups = new List<GameObject>();
	}

	public void AddGroup(string groupName)
	{
		GameObject newGroup = Instantiate(_groupPrefab, _scrollView.transform);
		newGroup.GetComponent<GroupUI>().InitializeGroup(groupName);
		newGroup.GetComponent<GroupUI>().OnGroupClick += (id) =>
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke(id);
		};

		_groups.Add(newGroup);
	}

	public void AddCart()
	{
		GameObject newGroup = Instantiate(_groupCartPrefab, _scrollView.transform);
		newGroup.GetComponent<CartGroupUI>().onGroupClick += (id) =>
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke(id);
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