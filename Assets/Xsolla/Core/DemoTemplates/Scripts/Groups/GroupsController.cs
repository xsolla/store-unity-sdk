using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupsController : MonoBehaviour
{
	public event Action<string> GroupSelectedEvent;

	[SerializeField] private GameObject groupPrefab;
	[SerializeField] private RectTransform scrollView;

	public List<IGroup> Groups { get; private set; }
	private readonly List<GameObject> _groupObjects = new List<GameObject>(); 

	private void Awake()
	{
		Groups = new List<IGroup>();
	}

	private void Start()
	{
		var hotKeys = gameObject.AddComponent<GroupsHotKeys>();
		hotKeys.ArrowDownKeyPressedEvent += SetPreviousGroup;
		hotKeys.ArrowUpKeyPressedEvent += SetNextGroup;
	}

	private void SetPreviousGroup()
	{
		IGroup group = GetSelectedGroup();
		int index = Groups.IndexOf(group) + 1;
		if (index >= Groups.Count)
			index = 0;
		group = Groups.ElementAt(index);
		group.Select();
		SelectGroup(group.Id);
	}

	private void SetNextGroup()
	{
		IGroup group = GetSelectedGroup();
		int index = Groups.IndexOf(group);
		if (index == 0)
		{
			index = Groups.Count - 1;
		}
		else
		{
			index--;
		}

		group = Groups.ElementAt(index);
		group.Select();
		SelectGroup(group.Id);
	}

	public GameObject AddGroup(string groupName)
	{
		if (Groups.Exists(group => group.Name == groupName))
			return _groupObjects.First(o => o.GetComponent<IGroup>().Name == groupName);
		var newGroupGameObject = Instantiate(groupPrefab, scrollView.transform);
		_groupObjects.Add(newGroupGameObject);
		newGroupGameObject.name =
			"Group_" +
			groupName.ToUpper().First() +
			groupName.Substring(1).Replace(" ", "").ToLower();

		var newGroup = newGroupGameObject.GetComponent<IGroup>();
		newGroup.Id = groupName;
		newGroup.Name = groupName;
		newGroup.OnGroupClick += SelectGroup;

		Groups.Add(newGroup);
		return newGroupGameObject;
	}

	public void SelectGroup(string groupId)
	{
		Groups.Where(g => g.Id != groupId).ToList().ForEach(g => g.Deselect());
		if (GroupSelectedEvent != null)
			GroupSelectedEvent.Invoke(groupId);
	}

	public void SelectDefault()
	{
		if (Groups.Any())
		{
			Groups.First().Select();
		}
	}

	public IGroup GetSelectedGroup()
	{
		var selected = Groups.Find(group => group.IsSelected());
		if (selected != null || !Groups.Any()) return selected;

		SelectDefault();
		selected = Groups.Find(group => group.IsSelected());
		return selected;
	}

	public void RemoveAll()
	{
		_groupObjects.ForEach(Destroy);
		_groupObjects.Clear();
		Groups.Clear();
	}
}
