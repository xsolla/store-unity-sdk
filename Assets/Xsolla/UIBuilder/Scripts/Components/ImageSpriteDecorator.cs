using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Sprite Decorator")]
	public class ImageSpriteDecorator : ThemeDecorator
	{
		[SerializeField] private Image _image;

		public Image Image
		{
			get => _image;
			set => _image = value;
		}

		public override void ApplyTheme(Theme theme)
		{
			if (Image && theme != null)
			{
				ValidateSpritePropertyId(theme);

				var prop = theme.GetSpriteProperty(PropertyId);
				if (prop != null)
				{
					Image.sprite = prop.Sprite;
				}
			}
		}

		protected override void CheckComponentExists()
		{
			AssignComponentIfNotExists(ref _image);
		}
	}
}