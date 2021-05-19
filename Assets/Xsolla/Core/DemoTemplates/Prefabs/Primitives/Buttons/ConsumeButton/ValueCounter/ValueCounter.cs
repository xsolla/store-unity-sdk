using System;
using UnityEngine;

public class ValueCounter : MonoBehaviour
{
	public event Action<int> ValueChanged;
	private int value;

	public int GetValue()
	{
		return value;
	}

	public void IncreaseValue(int delta)
	{
		value += delta;
		if (ValueChanged != null)
			ValueChanged.Invoke(value);
	}

	public void DecreaseValue(int delta)
	{
		if (delta > value)
			delta = value;
		value -= delta;
		if (ValueChanged != null)
			ValueChanged.Invoke(value);
	}

	public override string ToString()
	{
		return value.ToString();
	}

	public static ValueCounter operator ++(ValueCounter value)
	{
		value.IncreaseValue(1);
		return value;
	}

	public static ValueCounter operator --(ValueCounter value)
	{
		value.DecreaseValue(1);
		return value;
	}

	public static implicit operator int(ValueCounter value)
	{
		return value.GetValue();
	}
}
