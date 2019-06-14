using System;
using UnityEngine;
using UnityEngine.UI;

public class CartGroupUI : MonoBehaviour
{
	[SerializeField]
	Text CounterText;
	
	[SerializeField]
	Button _cart_group_Btn;

	public Action<string> OnGroupClick;
	
	void Awake()
	{
		_cart_group_Btn.onClick.AddListener(() => 
		{
			if (OnGroupClick != null)
				OnGroupClick.Invoke("CART");
		});
	}
	
	public void UpdateCounter(int counter)
	{
		// TODO
	}
}