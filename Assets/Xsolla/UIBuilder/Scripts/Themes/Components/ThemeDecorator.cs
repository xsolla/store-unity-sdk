using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class ThemeDecorator : MonoBehaviour
	{
		[SerializeField] private string _propertyId;

		[SerializeField] private DecoratorPointerOverrider _pointerOverrider;

		protected string PropertyId
		{
			get => _propertyId;
			set => _propertyId = value;
		}

		public DecoratorPointerOverrider PointerOverrider
		{
			get => _pointerOverrider;
			set => _pointerOverrider = value;
		}

		protected void Awake()
		{
			Init();
		}

		protected void OnEnable()
		{
			PointerOverrider.OnEnable();
			ThemesLibrary.CurrentChanged += OnCurrentThemeChanged;
			ApplyTheme(ThemesLibrary.Current);
		}

		protected void OnDisable()
		{
			PointerOverrider.OnDisable();
			ThemesLibrary.CurrentChanged -= OnCurrentThemeChanged;
		}

		protected void OnValidate()
		{
			ApplyTheme(ThemesLibrary.Current);
		}

		protected void Reset()
		{
			CheckComponentExists();

			if (PointerOverrider == null)
			{
				PointerOverrider = new DecoratorPointerOverrider();
			}
		}

		private void OnCurrentThemeChanged(Theme theme)
		{
			ApplyTheme(theme);
		}

		public abstract void ApplyTheme(Theme theme);

		protected abstract void Init();

		protected abstract void CheckComponentExists();

		protected void AssignComponentIfNotExists<T>(ref T value) where T : Component
		{
			if (!value)
			{
				value = GetComponent<T>();
			}
		}

		protected void ValidatePropertyId<T>(IEnumerable<T> properties) where T : ThemeProperty
		{
			if (string.IsNullOrEmpty(PropertyId))
			{
				var prop = properties.FirstOrDefault();
				PropertyId = prop != null ? prop.Id : string.Empty;
			}
		}
	}
}