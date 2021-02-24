using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Color Decorator", 100)]
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
				ValidatePropertyId(theme.Colors);
				ApplyColor(theme, PropertyId);

				PointerOverrider.ValidatePropertyId(theme.Colors);
			}
		}

		protected override void Init()
		{
			PointerOverrider.ApplyAction = ApplyColor;
			PointerOverrider.ResetAction = theme => ApplyColor(theme, PropertyId);
		}

		protected override void CheckComponentExists()
		{
			AssignComponentIfNotExists(ref _image);
		}

		private void ApplyColor(Theme theme, string id)
		{
			if (!Image)
				return;

			var prop = theme?.GetColorProperty(id);
			if (prop == null)
				return;

			Image.color = prop.Color;
		}
	}
}