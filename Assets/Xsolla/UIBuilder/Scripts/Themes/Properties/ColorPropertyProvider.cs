using UnityEngine;
using System;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ColorPropertyProvider : ThemePropertyProvider
	{
		public Color GetColor()
		{
			return ThemesLibrary.Current.GetColorProperty(Id).Color;
		}
	}
}