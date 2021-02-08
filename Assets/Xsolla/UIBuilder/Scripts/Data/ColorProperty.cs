using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ColorProperty : ThemeProperty
	{
		[SerializeField] private Color _color;

		public Color Color
		{
			get => _color;
			set => _color = value;
		}

		public ColorProperty()
		{
			Name = "New Color";
			Color = Color.white;
		}
	}
}