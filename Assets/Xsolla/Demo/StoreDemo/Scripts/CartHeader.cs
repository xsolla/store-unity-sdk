using System;
using UnityEngine;

public class CartHeader : MonoBehaviour
{
	[SerializeField]
	SimpleButton clearCartButton;

	public Action OnClearCart
	{
		set
		{
			clearCartButton.onClick = value;
		}
	}
}