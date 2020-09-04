using System;
using UnityEngine;
using UnityEngine.UI;

public class CartDiscountUI : MonoBehaviour
{
	[SerializeField]
	Text discountAmount;

	public void Initialize(float discount)
	{
		Debug.Log("Discount = " + discount);
		discountAmount.text = $"- {PriceFormatter.FormatPrice(discount)}";
	}
}