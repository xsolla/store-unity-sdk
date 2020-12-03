using UnityEngine;

namespace Xsolla.Demo
{
	public class SimpleTextButtonDisableable : SimpleTextButton
	{
		[SerializeField] Sprite disabledStateSprite = default;
		[SerializeField] Color disabledTextColor = default;

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
}
