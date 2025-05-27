using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class SpriteCacheHandler
	{
		public Sprite Sprite { get; set; }

		public List<Action<Sprite>> Callbacks { get; } = new();
	}
}