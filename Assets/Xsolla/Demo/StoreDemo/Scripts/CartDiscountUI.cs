using System;
using UnityEngine;
using UnityEngine.UI;

public class CartDiscountUI : MonoBehaviour
{
	[SerializeField]
	Text discountAmount;

	public void Initialize(float discount)
	{
		print(discount);
		discountAmount.text = "- $" + Math.Round(discount, 2, MidpointRounding.AwayFromZero).ToString("F2");
	}
}