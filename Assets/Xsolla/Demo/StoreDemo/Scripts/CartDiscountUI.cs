using System;
using UnityEngine;
using UnityEngine.UI;

public class CartDiscountUI : MonoBehaviour
{
	[SerializeField]
	Text discountAmount;

	public void Initialize(float discount)
	{
		discountAmount.text = "- " + Math.Round(discount, 2).ToString("F2") + " USD";
	}
}