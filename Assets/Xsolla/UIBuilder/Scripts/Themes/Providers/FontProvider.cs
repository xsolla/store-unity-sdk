using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class FontProvider : ThemePropertyProvider
	{
		public Font GetFont()
		{
			return ThemesLibrary.Current.GetFontProperty(Id).Font;
		}
	}
}