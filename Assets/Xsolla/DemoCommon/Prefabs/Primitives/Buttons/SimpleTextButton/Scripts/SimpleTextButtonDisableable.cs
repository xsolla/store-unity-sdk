using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class SimpleTextButtonDisableable : SimpleTextButton
	{
		[SerializeField] Sprite disabledStateSprite = default;
		[SerializeField] Color disabledTextColor = default;

		[Space]
		[SerializeField] private DecoratorPointerEvents DecoratorPointerEvents = default;

		[SerializeField] private Image DisableImage = default;
		[SerializeField] private SpritePropertyProvider NormalSpriteProp = default;
		[SerializeField] private SpritePropertyProvider DisabledSpriteProp = default;
		[SerializeField] private ColorPropertyProvider NormalImageColorProp = default;
		[SerializeField] private ColorPropertyProvider DisabledImageColorProp = default;

		[SerializeField] private Text DisableText = default;
		[SerializeField] private ColorPropertyProvider NormalTextColorProp = default;
		[SerializeField] private ColorPropertyProvider DisabledTextColorProp = default;

		public void Enable()
		{
			DisableImage.sprite = NormalSpriteProp.GetSprite();
			DisableImage.color = NormalImageColorProp.GetColor();
			DisableText.color = NormalTextColorProp.GetColor();

			DecoratorPointerEvents.IsMute = false;
			Unlock();
		}

		public void Disable()
		{
			Lock();
			DecoratorPointerEvents.IsMute = true;

			DisableImage.sprite = DisabledSpriteProp.GetSprite();
			DisableImage.color = DisabledImageColorProp.GetColor();
			DisableText.color = DisabledTextColorProp.GetColor();
		}
	}
}