using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartDiscountUI : MonoBehaviour
	{
		[SerializeField]
		Text discountAmount = default;

		public void Initialize(float discount)
		{
			Debug.Log("Discount = " + discount);
			discountAmount.text = $"- {PriceFormatter.FormatPrice(discount)}";
		}
	}
}
