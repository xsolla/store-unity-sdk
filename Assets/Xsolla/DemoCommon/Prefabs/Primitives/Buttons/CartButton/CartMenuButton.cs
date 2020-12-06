using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartMenuButton : SimpleTextButton
	{
		[SerializeField]
		Text counterText = default;
		[SerializeField]
		Image counterImage = default;

		[SerializeField]
		Sprite normalCounterSprite = default;
		[SerializeField]
		Sprite hoverCounterSprite = default;
		[SerializeField]
		Sprite pressedCounterSprite = default;

		[SerializeField]
		Color normalCounterTextColor = default;
		[SerializeField]
		Color hoverCounterTextColor = default;
		[SerializeField]
		Color pressedCounterTextColor = default;

		public string CounterText
		{
			get => counterText.text;
			set => counterText.text = value;
		}
	
		protected override void OnNormal()
		{
			base.OnNormal();

			SetImageSprite(counterImage, normalCounterSprite);
			counterText.color = normalCounterTextColor;
		}
	
		protected override void OnHover()
		{
			base.OnHover();
		
			SetImageSprite(counterImage, hoverCounterSprite);
			counterText.color = hoverCounterTextColor;
		}
	
		protected override void OnPressed()
		{
			base.OnPressed();
		
			SetImageSprite(counterImage, pressedCounterSprite);
			counterText.color = pressedCounterTextColor;
		}
	}
}
