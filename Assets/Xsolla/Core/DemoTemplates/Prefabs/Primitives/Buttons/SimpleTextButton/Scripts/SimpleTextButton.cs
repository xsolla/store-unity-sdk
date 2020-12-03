using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class SimpleTextButton : SimpleButtonLockDecorator
	{
		[SerializeField] Text buttonText = default;

		[SerializeField] Color normalTextColor = default;
		[SerializeField] Color hoverTextColor = default;
		[SerializeField] Color pressedTextColor = default;

		public string Text
		{
			get => buttonText.text;
			set => buttonText.text = value;
		}

		protected Text ButtonTextComponent => buttonText;

		private void SetButtonTextColor(Color color)
		{
			if(buttonText != null)
				buttonText.color = color;
		}
	
		protected override void OnNormal()
		{
			SetButtonTextColor(normalTextColor);
			base.OnNormal();
		}

		protected override void OnHover()
		{
			SetButtonTextColor(hoverTextColor);
			base.OnHover();
		}

		protected override void OnPressed()
		{
			SetButtonTextColor(pressedTextColor);
			base.OnPressed();
		}
	}
}
