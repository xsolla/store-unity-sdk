using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTextButton : SimpleButton
{
	[SerializeField]
	Text buttonText;
	
	[SerializeField]
	Color normalTextColor;
	[SerializeField]
	Color hoverTextColor;
	[SerializeField]
	Color pressedTextColor;
	
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
