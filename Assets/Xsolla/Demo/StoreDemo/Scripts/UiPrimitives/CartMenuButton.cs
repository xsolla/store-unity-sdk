using UnityEngine;
using UnityEngine.UI;

public class CartMenuButton : MenuButton
{
	[SerializeField]
	Text counterText;
	[SerializeField]
	Image counterImage;
	[SerializeField]
	Image cartImage;
	
	[SerializeField]
	Sprite normalCounterSprite;
	[SerializeField]
	Sprite selectedCounterSprite;
	[SerializeField]
	Sprite hoverCounterSprite;
	[SerializeField]
	Sprite pressedCounterSprite;
	
	[SerializeField]
	Sprite normalCartSprite;
	[SerializeField]
	Sprite selectedCartSprite;
	[SerializeField]
	Sprite hoverCartSprite;
	[SerializeField]
	Sprite pressedCartSprite;
	
	[SerializeField]
	Color normalCounterTextColor;
	[SerializeField]
	Color selectedCounterTextColor;
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

		counterImage.sprite = normalCounterSprite;
		cartImage.sprite = normalCartSprite;
		counterText.color = normalCounterTextColor;
	}
	
	protected override void OnHover()
	{
		base.OnHover();
		
		counterImage.sprite = hoverCounterSprite;
		cartImage.sprite = hoverCartSprite;
		counterText.color = hoverCounterTextColor;
	}
	
	protected override void OnPressed()
	{
		base.OnPressed();
		
		counterImage.sprite = pressedCounterSprite;
		cartImage.sprite = pressedCartSprite;
		counterText.color = pressedCounterTextColor;
	}
	
	protected override void OnSelected()
	{
		base.OnSelected();
		
		counterImage.sprite = selectedCounterSprite;
		cartImage.sprite = selectedCartSprite;
		counterText.color = selectedCounterTextColor;
	}
}