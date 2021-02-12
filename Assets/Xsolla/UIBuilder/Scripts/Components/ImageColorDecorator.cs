using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Color Decorator")]
	public class ImageColorDecorator : ThemeDecorator
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
				ValidateColorPropertyId(theme);

				var prop = theme.GetColorProperty(PropertyId);
				if (prop != null)
				{
					Image.color = prop.Color;
				}
			}
		}

		protected override void CheckComponentExists()
		{
			AssignComponentIfNotExists(ref _image);
		}
	}
}