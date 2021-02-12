using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class SpriteProperty : ThemeProperty
	{
		[SerializeField] private Sprite _sprite;

		public Sprite Sprite
		{
			get => _sprite;
			set => _sprite = value;
		}

		public SpriteProperty()
		{
			Name = "New Sprite";
		}
	}
}