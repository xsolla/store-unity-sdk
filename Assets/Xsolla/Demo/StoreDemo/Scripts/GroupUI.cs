using System;
using UnityEngine;

public class GroupUI : MonoBehaviour
{
	[SerializeField]
	MenuButton menuButton;

	public Action<string> OnGroupClick;

	void Awake()
	{
		menuButton.onClick = ((s) =>
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke(s);
		});
	}

	public void InitializeGroup(string name)
	{
		menuButton.Text = name;
	}
	
	public void Deselect(string groupName)
	{
		if (menuButton.Text != groupName)
		{
			menuButton.Deselect();
		}
	}
}