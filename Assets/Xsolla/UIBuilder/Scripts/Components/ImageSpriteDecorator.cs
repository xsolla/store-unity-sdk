using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Sprite Decorator", 100)]
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
				ValidatePropertyId(theme.Sprites);
				ApplySprite(theme, PropertyId);

				PointerOverrider.ValidatePropertyId(theme.Sprites);
			}
		}

		protected override void Init()
		{
			PointerOverrider.ApplyAction = ApplySprite;
			PointerOverrider.ResetAction = theme => ApplySprite(theme, PropertyId);
		}

		protected override void CheckComponentExists()
		{
			if (!Image)
			{
				Image = GetComponent<Image>();
			}

			if (PointerOverrider == null)
			{
				PointerOverrider = new DecoratorPointerOverrider();
			}
		}

		private void ApplySprite(Theme theme, string id)
		{
			if (!Image)
				return;

			var prop = theme.GetSpriteProperty(id);
			if (prop == null)
				return;

			Image.sprite = prop.Sprite;
		}
	}
}