using UnityEngine;
using UnityEngine.UI;

public class CartMenuButton : SimpleTextButton
{
	[SerializeField]
	Text counterText;
	[SerializeField]
	Image counterImage;
	
	[SerializeField]
	Sprite normalCounterSprite;
	[SerializeField]
	Sprite hoverCounterSprite;
	[SerializeField]
	Sprite pressedCounterSprite;

	[SerializeField]
	Color normalCounterTextColor;
	[SerializeField]
	Color hoverCounterTextColor;
	[SerializeField]
	Color pressedCounterTextColor;
	
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
