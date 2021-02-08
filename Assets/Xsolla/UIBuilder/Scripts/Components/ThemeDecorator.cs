using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class ThemeDecorator : MonoBehaviour
	{
		[SerializeField] private string _propertyId;

		protected string PropertyId
		{
			get => _propertyId;
			set => _propertyId = value;
		}

		protected void OnEnable()
		{
			ThemesLibrary.CurrentChanged += OnCurrentThemeChanged;
			ApplyTheme(ThemesLibrary.Current);
		}

		protected void OnDisable()
		{
			ThemesLibrary.CurrentChanged -= OnCurrentThemeChanged;
		}

		protected void OnValidate()
		{
			ApplyTheme(ThemesLibrary.Current);
		}

		protected void Reset()
		{
			CheckComponentExists();
		}

		private void OnCurrentThemeChanged(Theme theme)
		{
			ApplyTheme(theme);
		}

		public abstract void ApplyTheme(Theme theme);

		protected abstract void CheckComponentExists();

		protected void AssignComponentIfNotExists<T>(ref T value) where T : Component
		{
			if (!value)
			{
				value = GetComponent<T>();
			}
		}

		protected void ValidateColorPropertyId(Theme theme)
		{
			if (string.IsNullOrEmpty(PropertyId))
			{
				var prop = theme.Colors.FirstOrDefault();
				PropertyId = prop != null ? prop.Id : string.Empty;
			}
		}

		protected void ValidateSpritePropertyId(Theme theme)
		{
			if (string.IsNullOrEmpty(PropertyId))
			{
				var prop = theme.Sprites.FirstOrDefault();
				PropertyId = prop != null ? prop.Id : string.Empty;
			}
		}
	}
}