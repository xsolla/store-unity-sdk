using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class SpriteProvider : ThemePropertyProvider
	{
		public Sprite GetSprite()
		{
			return ThemesLibrary.Current.GetSpriteProperty(Id).Sprite;
		}
	}
}