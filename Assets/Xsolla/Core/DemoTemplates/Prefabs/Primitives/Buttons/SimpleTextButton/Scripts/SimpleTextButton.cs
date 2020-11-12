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
		get => buttonText.text;
		set => buttonText.text = value;
	}

	protected Text ButtonTextComponent => buttonText;

	protected override void OnNormal()
	{
		buttonText.color = normalTextColor;
		base.OnNormal();
	}

	protected override void OnHover()
	{
		buttonText.color = hoverTextColor;
		base.OnHover();
	}

	protected override void OnPressed()
	{
		buttonText.color = pressedTextColor;
		base.OnPressed();
	}
}