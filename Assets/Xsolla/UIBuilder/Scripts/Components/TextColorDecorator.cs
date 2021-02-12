using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Text Color Decorator")]
	public class TextColorDecorator : ThemeDecorator
	{
		[SerializeField] private Text _text;

		public Text Text
		{
			get => _text;
			set => _text = value;
		}

		public override void ApplyTheme(Theme theme)
		{
			if (Text && theme != null)
			{
				ValidateColorPropertyId(theme);

				var prop = theme.GetColorProperty(PropertyId);
				if (prop != null)
				{
					Text.color = prop.Color;
				}
			}
		}

		protected override void CheckComponentExists()
		{
			AssignComponentIfNotExists(ref _text);
		}
	}
}