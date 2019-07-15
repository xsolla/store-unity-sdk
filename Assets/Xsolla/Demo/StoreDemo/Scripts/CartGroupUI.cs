using System;
using UnityEngine;
using Xsolla.Core;

public class CartGroupUI : MonoBehaviour
{
	[SerializeField]
	CartMenuButton menuButton;
	
	int _counter;

	public Action<string> onGroupClick;
	
	void Awake()
	{
		menuButton.onClick = ((s) =>
		{
			if (onGroupClick != null)
				onGroupClick.Invoke(s);
		});

		menuButton.Text = Constants.CartGroupName;
	}

	public void Deselect(string groupName)
	{
		if (menuButton.Text != groupName)
		{
			menuButton.Deselect();
		}
	}
	
	public void IncreaseCounter(int value = 1)
	{
		_counter += value;
		menuButton.CounterText = _counter.ToString();
	}
	
	public void DecreaseCounter(int value = 1)
	{
		_counter -= value;
		menuButton.CounterText = _counter.ToString();
	}

	public void ResetCounter()
	{
		_counter = 0;
		menuButton.CounterText = _counter.ToString();
	}
}