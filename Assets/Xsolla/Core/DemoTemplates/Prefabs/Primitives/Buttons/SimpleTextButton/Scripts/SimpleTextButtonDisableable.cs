using UnityEngine;

public class SimpleTextButtonDisableable : SimpleTextButton
{
#pragma warning disable 0649
	[SerializeField] Sprite disabledStateSprite;
	[SerializeField] Color disabledTextColor;
#pragma warning restore 0649

	public void Disable()
	{
		base.Lock();
		base.SetImageSprite(disabledStateSprite);
		base.ButtonTextComponent.color = disabledTextColor;
	}

	public void Enable()
	{
		base.Unlock();
		base.OnNormal();
	}
}
