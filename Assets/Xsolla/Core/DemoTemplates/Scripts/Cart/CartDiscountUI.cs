using UnityEngine;
using UnityEngine.UI;

public class CartDiscountUI : MonoBehaviour
{
	[SerializeField]
	Text discountAmount;

	public void Initialize(float discount)
	{
		Debug.Log("Discount = " + discount);
		var discountAmountValue = string.Format("- {0}", PriceFormatter.FormatPrice(discount));
		discountAmount.text = discountAmountValue;
	}
}
