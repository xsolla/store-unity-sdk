using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartMenuButton : SimpleTextButton
	{
		[SerializeField] Text counterText;

		public string CounterText
		{
			get
			{
				return counterText.text;
			}
			set
			{
				counterText.text = value;
			}
		}
	}
}
