using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class FontProperty : ThemeProperty
	{
		[SerializeField] private Font _font;

		public Font Font
		{
			get => _font;
			set => _font = value;
		}

		public FontProperty()
		{
			Name = "New Font";
		}
	}
}