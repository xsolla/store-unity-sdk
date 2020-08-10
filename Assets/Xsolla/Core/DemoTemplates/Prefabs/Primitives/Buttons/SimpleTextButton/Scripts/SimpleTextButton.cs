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
		base.OnNormal();

		buttonText.color = normalTextColor;
	}

	protected override void OnHover()
	{
		base.OnHover();

		buttonText.color = hoverTextColor;
	}

	protected override void OnPressed()
	{
		base.OnPressed();

		buttonText.color = pressedTextColor;
	}
}