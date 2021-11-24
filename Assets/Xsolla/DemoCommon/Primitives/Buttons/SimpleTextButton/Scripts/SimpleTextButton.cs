using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class SimpleTextButton : SimpleButtonLockDecorator
	{
		[SerializeField] Text buttonText;

		public string Text
		{
			get
			{
				return buttonText.text;
			}
			set
			{
				buttonText.text = value;
			}
		}
	}
}
