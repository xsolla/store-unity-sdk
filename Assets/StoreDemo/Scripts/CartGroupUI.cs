using System;
using UnityEngine;
using UnityEngine.UI;

public class CartGroupUI : MonoBehaviour
{
	[SerializeField]
	GameObject counterPlaceholder;
	[SerializeField]
	Text counterText;

	int _counter;

	public Action<string> onGroupClick;
	
	void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() => 
		{
			if (onGroupClick != null)
			{
				onGroupClick.Invoke("CART");
			}
		});
	}
	
	public void IncreaseCounter(int value = 1)
	{
		_counter += value;
		
		if (_counter > 0)
		{
			counterPlaceholder.SetActive(true);
		}
		
		counterText.text = _counter.ToString();
	}
	
	public void DecreaseCounter(int value = 1)
	{
		_counter -= value;

		if (_counter == 0)
		{
			counterPlaceholder.SetActive(false);
		}
		else
		{
			counterText.text = _counter.ToString();
		}
	}

	public void ResetCounter()
	{
		_counter = 0;
		counterPlaceholder.SetActive(false);
	}
}