using UnityEngine;
using UnityEngine.UI;

public class SimpleTextButton : SimpleButtonLockDecorator
{
	
	[SerializeField] Text buttonText;

	[SerializeField] Color normalTextColor;
	[SerializeField] Color hoverTextColor;
	[SerializeField] Color pressedTextColor;

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

	protected Text ButtonTextComponent
	{
		get
		{
			return buttonText;
		}
	}

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
