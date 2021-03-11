using UnityEngine;
using System;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ColorProvider : ThemePropertyProvider
	{
		public Color GetColor()
		{
			return ThemesLibrary.Current.GetColorProperty(Id).Color;
		}
	}
}