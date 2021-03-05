using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Text Font Decorator", 100)]
	public class TextFontDecorator : ThemeDecorator
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
				ValidatePropertyId(theme.Fonts);
				ApplyFont(theme, PropertyId);

				PointerOverrider.ValidatePropertyId(theme.Fonts);
			}
		}

		protected override void Init()
		{
			PointerOverrider.ApplyAction = ApplyFont;
			PointerOverrider.ResetAction = theme => ApplyFont(theme, PropertyId);
		}

		protected override void CheckComponentExists()
		{
			AssignComponentIfNotExists(ref _text);
		}

		private void ApplyFont(Theme theme, string id)
		{
			if (!Text)
				return;

			var prop = theme?.GetFontProperty(id);
			if (prop == null)
				return;

			Text.font = prop.Font;
		}
	}
}