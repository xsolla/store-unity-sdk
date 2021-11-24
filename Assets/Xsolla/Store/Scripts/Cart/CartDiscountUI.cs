using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartDiscountUI : MonoBehaviour
	{
		[SerializeField]
		Text discountAmount;

		public void Initialize(float discount)
		{
			Debug.Log("Discount = " + discount);
			discountAmount.text = string.Format("- {0}", PriceFormatter.FormatPrice(discount));
		}
	}
}
