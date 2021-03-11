using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Text Color Decorator", 100)]
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
			AssignComponentIfNotExists(ref _text);
		}

		private void ApplyColor(Theme theme, string id)
		{
			if (!Text)
				return;

			var prop = theme?.GetColorProperty(id);
			if (prop == null)
				return;

			Text.color = prop.Color;
		}
	}
}