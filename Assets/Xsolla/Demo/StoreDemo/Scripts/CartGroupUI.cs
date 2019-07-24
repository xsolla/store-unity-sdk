using System;
using UnityEngine;

public class CartGroupUI : MonoBehaviour, IGroup
{
	[SerializeField]
	CartMenuButton menuButton;
	
	int _counter;
	
	void Awake()
	{
		menuButton.onClick = ((s) =>
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke(s);
		});
	}
	
	public string Name
	{
		get { return menuButton.Text; }
		set { menuButton.Text = value; }
	}

	public Action<string> OnGroupClick { get; set; }

	public void Select()
	{
		menuButton.Select();
	}

	public void Deselect()
	{
		menuButton.Deselect();
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