using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class SimpleTextButtonDisableable : SimpleTextButton
	{
		[Space]
		//[SerializeField] private DecoratorPointerEvents DecoratorPointerEvents;

		[SerializeField] private Image DisableImage;
		[SerializeField] private Sprite NormalSpriteProp;
		[SerializeField] private Sprite DisabledSpriteProp;
		[SerializeField] private Color NormalImageColorProp;
		[SerializeField] private Color DisabledImageColorProp;

		[SerializeField] private Text DisableText;
		[SerializeField] private Color NormalTextColorProp;
		[SerializeField] private Color DisabledTextColorProp;

		public void Enable()
		{
			DisableImage.sprite = NormalSpriteProp;
			DisableImage.color = NormalImageColorProp;
			DisableText.color = NormalTextColorProp;

			//DecoratorPointerEvents.IsMute = false;
			Unlock();
		}

		public void Disable()
		{
			Lock();
			//DecoratorPointerEvents.IsMute = true;

			DisableImage.sprite = DisabledSpriteProp;
			DisableImage.color = DisabledImageColorProp;
			DisableText.color = DisabledTextColorProp;
		}
	}
}
